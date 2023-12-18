using Environment.Model;
using Environment.Model.Module;
using Environment.Model.Packet;
using Environment.Service.Interface;
using MathNet.Numerics;
using System.Collections.Concurrent;
using System.IO.Ports;

namespace Environment.Base
{
    public class BaseEnvironment
    {
        public List<string> Ports = new List<string>();
        public List<SerialPort> SerialPorts = new List<SerialPort>();
        public List<NodeDevice> Devices = new List<NodeDevice>();
        public List<ModuleObject> ModuleObjects = new List<ModuleObject>();
        public string portClicked { get; set; }
        public object lockObjectSetParams = new object();
        //public Thread Collision { get; set; }

        private readonly ICommunication communication;

        public BaseEnvironment(ICommunication communication)
        {
            EnvState.PreProgramStatus = PROGRAM_STATUS.IDLE;
            EnvState.ProgramStatus = PROGRAM_STATUS.IDLE;
            this.communication = communication;
            SetUp();
        }

        public void SetUp()
        {
            var ports = SerialPort.GetPortNames();
            Ports = ports.ToList();
        }
        /*
         * ============== Functions for interraction with SerialPorts including: OPEN, CLOSE =============================== 
         */
        public void StartPort(string port)
        {
            var serialPort = SerialPorts.FirstOrDefault(x => x.PortName == port);
            if (serialPort == null)
            {
                serialPort = new SerialPort(port, 115200);
                SerialPorts.Add(serialPort);
                serialPort.ReadTimeout = 1000;
            }
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
            }

        }
        public void ClosePort(string port)
        {
            foreach (var serialport in SerialPorts)
            {
                if (serialport.PortName == port)
                {
                    serialport.Close();
                }
            }
        }
        /*
        * ================= Functions for first connection including: ACTIVE_HARDWARE, READCONFIG, CONFIG ======================
        */
        // Description: connect hardware
        public void ActiveHardwareDevice(string port)
        {
            var serialport = SerialPorts.FirstOrDefault(e => e.PortName == port);
            if (serialport != null)
            {
                byte[] stringActive = Helper.CmdActiveHardware();
                if (stringActive != null)
                {
                    serialport.Write(stringActive, 0, stringActive.Length);
                }

                return;
            }
        }
        /*
         * Description: read config from hardware
         * Output: id: string + module type: string
        */
        public PacketTransmit ExecuteReadConfigFromHardware(string port)
        {
            var serialport = SerialPorts.FirstOrDefault(s => s.PortName == port);
            if (serialport != null)
            {
                return Helper.SendCmdGetConfigFromHardware(serialport);
            }

            return null;
        }
        // Description: send config to hardware
        public bool ExecuteConfigToHardware(string port, string module, string id, string baudrate)
        {
            var serialport = SerialPorts.FirstOrDefault(s => s.PortName == port);
            // data is id module
            //return true;
            if (serialport != null)
            {
                if (module == ModuleObjectType.LORA)
                {
                    byte moduleType = 0x01;
                    byte[] data = new byte[3];
                    data[0] = Convert.ToByte(id);
                    data[1] = Helper.ConvertSpeedrate(baudrate);
                    data[2] = PacketTransmit.ENDBYTE;

                    return Helper.SendCmdConfigToHardware(serialport, moduleType, data);
                }
                else if (module == ModuleObjectType.ZIGBEE)
                {
                    byte moduleType = 0x02;
                    byte[] data = new byte[3];
                    data[0] = Convert.ToByte(id);
                    data[1] = Helper.ConvertSpeedrate(baudrate);
                    data[2] = PacketTransmit.ENDBYTE;

                    return Helper.SendCmdConfigToHardware(serialport, moduleType, data);
                }
            }
            return false;
        }
        /*
         * ======================================================================================================================
         */

        /*
         * ====================== Running part includes: PROGRAM_STATUS.RUN, and other small FUNCTIONS helping PROGRAM_STATUS.RUN ============================= 
         */
        // First we create ports initial and read data from that port, then push it into queueIn.
        public void createSerialPortInitial()
        {
            foreach (var serialport in SerialPorts)
            {

                if (!serialport.IsOpen)
                {
                    serialport.Open();
                    if (EnvState.PreProgramStatus == PROGRAM_STATUS.IDLE)
                    {
                        var device = new NodeDevice()
                        {
                            serialport = serialport,
                            existReadThread = false,
                        };
                        foreach (var module in ModuleObjects)
                        {
                            if (module.port == serialport.PortName)
                            {
                                device.moduleObject = module;
                            }
                        }

                        Devices.Add(device);
                    }

                }
            }
            foreach (NodeDevice hw in Devices)
            {
                if (hw.existReadThread == false)
                {
                    hw.readDataFromHardware = new Thread(() => readData(hw.serialport));
                    hw.readDataFromHardware.Name = "readData";
                    hw.readDataFromHardware.Start();
                    hw.existReadThread = true;
                }
            }
        }
        public void ResetParamsForDevice()
        {
            foreach (var hw in Devices)
            {
                var m_obj = ModuleObjects.FirstOrDefault(x => x.id == hw.moduleObject.id);
                if (m_obj != null)
                {
                    lock (lockObjectSetParams)
                    {
                        hw.moduleObject.parameters = m_obj.parameters;
                    }
                }
            }
        }
        private void readData(SerialPort sender)
        {
            while (EnvState.ProgramStatus == PROGRAM_STATUS.RUN)
            {
                byte[] buffer = Helper.GetDataFromHardware(sender);
                sender.DiscardInBuffer();
                if (buffer.Length > 0)
                {
                    addToQueueIn(buffer, sender);
                }

            }
            if (EnvState.ProgramStatus == PROGRAM_STATUS.PAUSE)
            {
                sender.Close();
                communication.sendMessageIsPause();
                // update exist read thread
                foreach (var hw in Devices)
                {
                    if (hw.serialport.PortName == sender.PortName)
                    {
                        hw.existReadThread = false;
                    }
                }
                Pause();
            }
            if (EnvState.ProgramStatus == PROGRAM_STATUS.IDLE)
            {
                communication.sendMessageIsStop();
            }
        }
        private void addToQueueIn(byte[] buffer, SerialPort sender)
        {
            var packet = Helper.HandleMessFromHardware(buffer);
            if (packet != null)
            {
                // check type of packet, if it is data packet, then add to queue in, else if it is change mode packet, then change mode

                if (packet.cmdWord == PacketTransmit.SENDDATA)
                {
                    foreach (var hardware in Devices)
                    {
                        if (hardware.serialport.PortName == sender.PortName)
                        {
                            if (hardware.moduleObject.type == ModuleObjectType.LORA)
                            {
                                var loraParameters = new LoraParameterObject();
                                lock (lockObjectSetParams)
                                {
                                    loraParameters = (LoraParameterObject)hardware.moduleObject.parameters;
                                }

                                DataProcessed data = new DataProcessed(loraParameters.FixedMode, packet.data);
                                hardware.packetQueueIn.Enqueue(data);
                            }
                            else if (hardware.moduleObject.type == ModuleObjectType.ZIGBEE)
                            {
                                var zigbeeParameters = new ZigbeeParameterObject();
                                lock (lockObjectSetParams)
                                {
                                    zigbeeParameters = (ZigbeeParameterObject)hardware.moduleObject.parameters;
                                }

                                DataProcessed data = new DataProcessed(packet.data);
                                hardware.packetQueueIn.Enqueue(data);
                            }

                        }
                    }
                }
                else if (packet.cmdWord == PacketTransmit.CHANGEMODE)
                {
                    int mode = packet.data[0];
                    foreach (var hardware in Devices)
                    {
                        if (hardware.serialport.PortName == sender.PortName)
                        {
                            if (hardware.moduleObject.type == ModuleObjectType.LORA)
                            {
                                lock (hardware.lockObjectChangeMode)
                                {
                                    hardware.mode = mode;
                                }

                                communication.deviceChangeMode(hardware.mode, hardware.moduleObject.id);
                                return;
                            }
                        }
                    }
                }

            }
        }
        public void RunProgram()
        {
            /*transferDataToView = new Thread(transferInPacketToView);
            transferDataToView.Start();*/
            foreach (var hw in Devices)
            {
                if(hw.moduleObject.type == ModuleObjectType.LORA)
                    hw.mode = 0;
                var moduleObject = ModuleObjects.FirstOrDefault(x => x.port == hw.serialport.PortName);
                if (moduleObject != null)
                {
                    hw.mode = 0;
                    communication.deviceChangeMode(0, hw.moduleObject.id);
                    hw.transferDataIn = new Thread(() => transferDataToDestinationDevice(hw, moduleObject));
                    hw.transferDataIn.Name = "datain";
                    hw.transferDataIn.Start();
                    hw.transferDataOut = new Thread(() => transferDataToHardware(hw, moduleObject));
                    hw.transferDataOut.Name = "dataout";
                    hw.transferDataOut.Start();
                }
            }
            communication.sendMessageIsRunning();
        }
        // transfer data from queue in to destination device
        private void transferDataToDestinationDevice(NodeDevice module, ModuleObject moduleObject)
        {
            while (EnvState.ProgramStatus == PROGRAM_STATUS.RUN)
            {
                if(module.moduleObject.type == ModuleObjectType.LORA)
                {
                    if (module.mode != NodeDevice.MODE_POWERSAVING && module.mode != NodeDevice.MODE_SLEEP)
                    {
                        if (module.packetQueueIn.TryDequeue(out DataProcessed packet))
                        {
                            communication.showQueueReceivedFromHardware(new PacketSendTransferToView()
                            {
                                type = "in",
                                portName = module.serialport.PortName,
                                packet = packet,
                            }, portClicked);
                            var inter_packet = ExecuteTransferDataToQueueOut(module.mode, packet, moduleObject);
                            if (inter_packet != null)
                            {
                                PushPackageIntoDestinationDevice(inter_packet, moduleObject);
                            }
                        }
                    }
                }
                else if(module.moduleObject.type == ModuleObjectType.ZIGBEE)
                {
                    if (module.packetQueueIn.TryDequeue(out DataProcessed packet))
                    {
                        communication.showQueueReceivedFromHardware(new PacketSendTransferToView()
                        {
                            type = "in",
                            portName = module.serialport.PortName,
                            packet = packet,
                        }, portClicked);
                        var inter_packet = ExecuteTransferDataToQueueOut(module.mode, packet, moduleObject);
                        if (inter_packet != null)
                        {
                            PushPackageIntoDestinationDevice(inter_packet, moduleObject);
                        }
                    }
                }
                
                
            }
            while (EnvState.ProgramStatus == PROGRAM_STATUS.PAUSE)
            {
                Pause();
                communication.sendMessageIsPause();
                break;
            }
            while (EnvState.ProgramStatus == PROGRAM_STATUS.IDLE)
            {
                communication.sendMessageIsStop();
            }
        }
        // Execute service transfer data from queue in( caculated delay time, preamble code, packet loss, conlision,...) then add to queue out
        private InternalPacket ExecuteTransferDataToQueueOut(int mode, DataProcessed packet, ModuleObject moduleObject)
        {
            if (moduleObject.type == ModuleObjectType.LORA)
            {
                var parameter = new LoraParameterObject();
                lock (lockObjectSetParams)
                {
                    parameter = (LoraParameterObject)moduleObject.parameters;
                }

                /*double range = CaculateService.computeRange(parameter.Power);
                double distance = CaculateService.computeDistance2Device(moduleObject, );*/
                if (parameter.FixedMode == FixedMode.BROARDCAST)
                {
                    packet.address = parameter.Address;
                    packet.channel = parameter.Channel;
                }
                switch (mode)
                {
                    case NodeDevice.MODE_NORMAL:
                        return new InternalPacket()
                        {
                            packet = packet,
                            sourceModule = moduleObject,
                            DelayTime = CaculateService.caculateDelayTime(parameter.AirRate, packet.data, "", parameter.FEC),
                        };
                    case NodeDevice.MODE_WAKEUP:
                        var preambleCode = Helper.generatePreamble(Convert.ToInt32(parameter.WORTime));
                        return new InternalPacket()
                        {
                            packet = packet,
                            sourceModule = moduleObject,
                            PreambleCode = preambleCode,
                            DelayTime = CaculateService.caculateDelayTime(parameter.AirRate, packet.data, preambleCode, parameter.FEC),
                        };
                }
            }
            else if(moduleObject.type == ModuleObjectType.ZIGBEE)
            {
                var parameter = new ZigbeeParameterObject();
                lock (lockObjectSetParams)
                {
                    parameter = (ZigbeeParameterObject)moduleObject.parameters;
                }

                if(parameter.TransmitMode == TransmitMode.BROADCAST)
                {
                    packet.channel = parameter.Channel;
                }
                else if(parameter.TransmitMode == TransmitMode.POINT_TO_POINT)
                {
                    packet.address = parameter.Address;
                    packet.channel = parameter.Channel;
                }

                return new InternalPacket()
                {
                    packet = packet,
                    sourceModule = moduleObject,
                    DelayTime = CaculateService.caculateDelayTime(parameter.AirRate, packet.data, "", ""),
                };
            }
            return null;
        }
        // Push package into destination device
        private void PushPackageIntoDestinationDevice(InternalPacket packet, ModuleObject moduleObject)
        {
            if (moduleObject.type == ModuleObjectType.LORA)
            {
                var loraParameters = new LoraParameterObject();
                lock (lockObjectSetParams)
                {
                    loraParameters = (LoraParameterObject)moduleObject.parameters;
                }

                foreach (var hw in Devices)
                {
                    if (hw.moduleObject.type == ModuleObjectType.LORA)
                    {
                        var tmp_packet = new InternalPacket()
                        {
                            packet = packet.packet,
                            DelayTime = packet.DelayTime,
                            PreambleCode = packet.PreambleCode,
                            RSSI = packet.RSSI,
                            PathLoss = packet.PathLoss,
                            SNR = packet.SNR,
                            Distance = packet.Distance,
                            sourceModule = packet.sourceModule,
                            receivedModule = hw.moduleObject,
                            timeUTC = DateTime.Now.ToLocalTime().ToString(),
                            timeMilisecond = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()
                        };

                        tmp_packet.Distance = CaculateService.computeDistance2Device(moduleObject, hw.moduleObject).ToString("F3");
                        tmp_packet.RSSI = CaculateService.computeRSSI(moduleObject, hw.moduleObject).ToString("F3");
                        if (loraParameters.FixedMode == FixedMode.BROARDCAST) // broadcast
                        {
                            if (hw.moduleObject.parameters is LoraParameterObject)
                            {
                                var hw_loraParameters = (LoraParameterObject)hw.moduleObject.parameters;
                                if (hw_loraParameters.Channel == tmp_packet.packet.channel && hw_loraParameters.Address != tmp_packet.packet.address)
                                {


                                    // check mode of destination device
                                    if (hw.mode == NodeDevice.MODE_NORMAL || hw.mode == NodeDevice.MODE_WAKEUP)
                                    {
                                        Task task = Task.Run(async () =>
                                        {
                                            await Task.Delay(Convert.ToInt32(tmp_packet.DelayTime));

                                            // Execute work: enqueue and handle collision
                                            await createTransmittionAsync(hw, tmp_packet);
                                        });
                                    }
                                    else if (hw.mode == NodeDevice.MODE_POWERSAVING)
                                    {
                                        // check preamble code
                                        if (tmp_packet.PreambleCode != null)
                                        {
                                            Task task = Task.Run(async () =>
                                            {
                                                await Task.Delay(Convert.ToInt32(tmp_packet.DelayTime));

                                                // Execute work: enqueue and handle collision
                                                await createTransmittionAsync(hw, tmp_packet);
                                            });

                                        }
                                    }
                                }
                            }
                        }
                        else // fixed
                        {
                            if (hw.moduleObject.parameters is LoraParameterObject)
                            {
                                var hw_loraParameters = new LoraParameterObject();
                                lock (lockObjectSetParams)
                                {
                                    hw_loraParameters = (LoraParameterObject)hw.moduleObject.parameters;
                                }

                                if (hw_loraParameters.Address == tmp_packet.packet.address && hw_loraParameters.Channel == tmp_packet.packet.channel)
                                {
                                    tmp_packet.Distance = CaculateService.computeDistance2Device(moduleObject, hw.moduleObject).ToString("F3");
                                    tmp_packet.RSSI = CaculateService.computeRSSI(moduleObject, hw.moduleObject).ToString("F3");
                                    // check mode of destination device
                                    if (hw.mode == NodeDevice.MODE_NORMAL || hw.mode == NodeDevice.MODE_WAKEUP)
                                    {
                                        Task task = Task.Run(async () =>
                                        {
                                            await Task.Delay(Convert.ToInt32(tmp_packet.DelayTime));

                                            // Execute work: enqueue and handle collision
                                            await createTransmittionAsync(hw, tmp_packet);
                                        });
                                    }
                                    else if (hw.mode == NodeDevice.MODE_POWERSAVING)
                                    {
                                        // check preamble code
                                        if (tmp_packet.PreambleCode != null)
                                        {
                                            Task task = Task.Run(async () =>
                                            {
                                                await Task.Delay(Convert.ToInt32(tmp_packet.DelayTime));

                                                // Execute work: enqueue and handle collision
                                                await createTransmittionAsync(hw, tmp_packet);
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (moduleObject.type == "zigbee")
            {
                var zigbeeParameters = new ZigbeeParameterObject();
                lock (lockObjectSetParams)
                {
                    zigbeeParameters = (ZigbeeParameterObject)moduleObject.parameters;
                }
                // check mode broadcast or point to point
                if (zigbeeParameters.TransmitMode == TransmitMode.BROADCAST)
                {
                    foreach (var hw in Devices)
                    {
                        if (hw.moduleObject.type == ModuleObjectType.ZIGBEE)
                        {
                            if (zigbeeParameters.Channel == packet.packet.channel)
                            {
                                // check mode of destination device
                                packet.Distance = CaculateService.computeDistance2Device(moduleObject, hw.moduleObject).ToString("F3");
                                packet.RSSI = CaculateService.computeRSSI(moduleObject, hw.moduleObject).ToString("F3");
                                if (hw.mode == NodeDevice.MODE_NORMAL || hw.mode == NodeDevice.MODE_WAKEUP)
                                {
                                    Task task = Task.Run(async () =>
                                    {
                                        await Task.Delay(Convert.ToInt32(packet.DelayTime));

                                        // Execute work: enqueue and handle collision
                                        await createTransmittionAsync(hw, packet);
                                    });
                                }
                                else if (hw.mode == NodeDevice.MODE_POWERSAVING)
                                {
                                    // check preamble code
                                    if (packet.PreambleCode != null)
                                    {
                                        Task task = Task.Run(async () =>
                                        {
                                            await Task.Delay(Convert.ToInt32(packet.DelayTime));

                                            // Execute work: enqueue and handle collision
                                            await createTransmittionAsync(hw, packet);
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                else if (zigbeeParameters.TransmitMode == TransmitMode.POINT_TO_POINT)
                {
                    foreach (var hw in Devices)
                    {
                        if (hw.moduleObject.type == ModuleObjectType.ZIGBEE)
                        {
                            if (zigbeeParameters.Address == packet.packet.address && zigbeeParameters.Channel == packet.packet.channel)
                            {
                                // check mode of destination device
                                packet.Distance = CaculateService.computeDistance2Device(moduleObject, hw.moduleObject).ToString("F3");
                                packet.RSSI = CaculateService.computeRSSI(moduleObject, hw.moduleObject).ToString("F3");
                                Task task = Task.Run(async () =>
                                {
                                    await Task.Delay(Convert.ToInt32(packet.DelayTime));

                                    // Execute work: enqueue and handle collision
                                    await createTransmittionAsync(hw, packet);
                                });
                            }
                        }
                    }
                }
            }
        }
        //Create transmittion with checking collision when pushing data into destinationQueue
        private async Task createTransmittionAsync(NodeDevice desHW, InternalPacket packet)
        {
            if (desHW.moduleObject.coveringAreaRange > Double.Parse(packet.Distance))
            {
                lock (desHW.lockObjecCollision)
                {
                    desHW.flagDataIn++;
                }
                await caculateCollisionAsync(desHW, packet);
            }
            else
            {
                Task outRange = Task.Run(() => PushIntoErrorQueue(desHW, packet, ERROR_TYPE.OUT_OF_RANGE));
            }

        }
        private async Task caculateCollisionAsync(NodeDevice desHW, InternalPacket packet)
        {
            await Task.Delay(7);
            lock (desHW.lockObjecCollision)
            {
                if (desHW.flagDataIn > 1)
                {
                    Task collision = Task.Run(() => PushIntoErrorQueue(desHW, packet, ERROR_TYPE.COLLIDED));
                }
                else
                {
                    Task sendToDestination = Task.Run(() => PushIntoDestinationDeviceQueue(desHW, packet));
                }
            }
        }
        private void PushIntoErrorQueue(NodeDevice desHW, InternalPacket packet, int typeError)
        {
            if (typeError == ERROR_TYPE.COLLIDED)
            {
                lock (desHW.lockObjecCollision)
                {
                    desHW.flagDataIn--;
                }
            }
            packet.typeError = typeError;
            desHW.errorPackets.Enqueue(packet);
        }
        private void PushIntoDestinationDeviceQueue(NodeDevice desHW, InternalPacket packet)
        {
            lock (desHW.lockObjecCollision)
            {
                desHW.flagDataIn--;
            }
            desHW.packetQueueOut.Enqueue(packet);
        }
        // transfer data from queue out to hardware. Delay time is caculated by CaculateService then send to hardware
        private void transferDataToHardware(NodeDevice module, ModuleObject moduleObject)
        {
            while (EnvState.ProgramStatus == PROGRAM_STATUS.RUN)
            {
                if (module.mode != NodeDevice.MODE_POWERSAVING && module.mode != NodeDevice.MODE_SLEEP)
                {
                    if (module.packetQueueOut.TryDequeue(out InternalPacket packet))
                    {

                        /*create task to delay time and after that send packet to hardware
                         * To do: caculate delay time
                         */
                        communication.showQueueReceivedFromOtherDevice(new PacketReceivedTransferToView()
                        {
                            type = "out",
                            packet = packet,
                        }, portClicked);

                        // format packet before send, follow protocol
                        module.serialport.DiscardOutBuffer();
                        PacketTransmit packetTransmit = Helper.formatDataFollowProtocol(PacketTransmit.SENDDATA, packet.packet.data);
                        byte[] data = packetTransmit.getPacket();
                        module.serialport.Write(data, 0, data.Length);
                    }
                    if (module.errorPackets.TryDequeue(out InternalPacket errPacket))
                    {
                        communication.showQueueError(new PacketReceivedTransferToView()
                        {
                            type = "error",
                            packet = errPacket,
                        }, portClicked);
                    }

                }

            }
            while (EnvState.ProgramStatus == PROGRAM_STATUS.PAUSE)
            {
                Pause();
                communication.sendMessageIsPause();
                break;
            }
            while (EnvState.ProgramStatus == PROGRAM_STATUS.IDLE)
            {
                communication.sendMessageIsStop();
            }
        }
        //Pause program ======
        public void Pause()
        {
            /* foreach (var hw in Devices)
             {
                 hw.serialport.Close();
             }*/
        }

        // Stop program =====
        public void Stop()
        {
            foreach (var hw in Devices)
            {
                /*hw.readDataFromHardware.Join();
                hw.transferDataIn.Join();
                hw.transferDataOut.Join();*/
                hw.serialport.Close();
            }
        }
        /*//Function listen from hardware
        private string listenConfigFromHardware(SerialPort serialPort)
        {
            EnvState.ProgramStatus = PROGRAM_STATUS.IDLE;
            foreach (var hw in Devices)
            {
                hw.serialport.Close();
            }
        }




        /*public void createThreadRunning()
        {
        }*/




    }
}
