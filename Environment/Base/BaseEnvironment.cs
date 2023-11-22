using Environment.Model;
using Environment.Model.Module;
using Environment.Model.Packet;
using Environment.Service.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Base
{
    public class BaseEnvironment
    {
        public readonly int IDLE = 0;
        public readonly int RUN = 1;
        public readonly int STOP = 2;
        public int State;

        public List<string> Ports = new List<string>();
        public List<SerialPort> SerialPorts = new List<SerialPort>();
        public List<NodeDevice> Devices = new List<NodeDevice>();
        public List<ModuleObject> ModuleObjects = new List<ModuleObject>();
        //public Thread Collision { get; set; }

        private readonly ICommunication communication;

        public BaseEnvironment(ICommunication communication)
        {
            State = IDLE;
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
            }
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
            }
            SerialPorts.Add(serialPort);
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
            return true;
            if (serialport != null)
            {
                if (module == ModuleObject.LORA)
                {
                    byte moduleType = 0x01;
                    byte[] data = new byte[3];
                    data[0] = Convert.ToByte(id);
                    data[1] = Helper.ConvertSpeedrate(baudrate);
                    data[2] = PacketTransmit.ENDBYTE;

                    return Helper.SendCmdConfigToHardware(serialport, moduleType, data);
                }
                else if (module == ModuleObject.ZIGBEE)
                {
                    /*                    var data = Helper.GenerateDataConfigZigbee(id, baudrate);
                                        return Helper.SendCmdConfigToHardware(serialport, module, data);*/
                }
            }
            return false;
        }
        /*
         * ======================================================================================================================
         */

        /*
         * ====================== Running part includes: RUN, and other small FUNCTIONS helping RUN ============================= 
         */
        // First we create ports initial and read data from that port, then push it into queueIn.
        public void createSerialPortInitial()
        {
            foreach (var serialport in SerialPorts)
            {

                if (!serialport.IsOpen)
                {
                    serialport.Open();
                    var device = new NodeDevice()
                    {
                        serialport = serialport,
                        mode = NodeDevice.MODE_SLEEP,

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
        private void readData(SerialPort sender)
        {
            while (State == RUN)
            {
                byte[] buffer = Helper.GetDataFromHardware(sender);
                if (buffer.Length > 0)
                {
                    addToQueueIn(buffer, sender);
                }
                
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
                            if(hardware.moduleObject.type == ModuleObject.LORA)
                            {
                                var loraParameters = (LoraParameterObject)hardware.moduleObject.parameters;
                                DataProcessed data = new DataProcessed(loraParameters.FixedMode, packet.data);
                                hardware.packetQueueIn.Enqueue(data);
                            }

                        }
                    }
                }
                else if (packet.cmdWord == PacketTransmit.CHANGEMODE)
                {
                    foreach (var hardware in Devices)
                    {
                        if (hardware.serialport.PortName == sender.PortName)
                        {
                            hardware.mode = packet.data[0];
                            communication.deviceChangeMode(hardware.mode, hardware.serialport.PortName);
                            return;
                        }
                    }
                }

                /*                DataProcessed dataProcessed = new DataProcessed(packet.data);

                                foreach (var hardware in Devices)
                                {
                                    if (hardware.serialport.PortName == sender.PortName)
                                    {
                                        hardware.packetQueueIn.Enqueue(dataProcessed);
                                    }
                                }*/
            }
        }
        public void RunProgram()
        {
            /*transferDataToView = new Thread(transferInPacketToView);
            transferDataToView.Start();*/
            foreach (var hw in Devices)
            {
                hw.mode = 0;
                var moduleObject = ModuleObjects.FirstOrDefault(x => x.port == hw.serialport.PortName);
                if (moduleObject != null)
                {
                    hw.readDataFromHardware = new Thread(() => readData(hw.serialport));
                    hw.readDataFromHardware.Start();
                    hw.transferDataIn = new Thread(() => transferDataToDestinationDevice(hw.mode, hw.serialport, hw.packetQueueIn, moduleObject));
                    hw.transferDataIn.Start();
                    hw.transferDataOut = new Thread(() => transferDataToHardware(hw.mode, hw.serialport, hw.packetQueueOut, moduleObject));
                    hw.transferDataOut.Start();
                }
            }
        }
        // transfer data from queue in to destination device
        private void transferDataToDestinationDevice(int mode, SerialPort serialPort, ConcurrentQueue<DataProcessed> packetQueue, ModuleObject moduleObject)
        {
            while (State == RUN)
            {
                if (mode != NodeDevice.MODE_POWERSAVING && mode != NodeDevice.MODE_SLEEP)
                {
                    if (packetQueue.TryDequeue(out DataProcessed packet))
                    {
                        communication.showQueueReceivedFromHardware(new PacketTransferToView()
                        {
                            portName = serialPort.PortName,
                            packet = packet,
                        });
                        var inter_packet = ExecuteTransferDataToQueueOut(mode, packet, moduleObject);
                        if (inter_packet != null)
                        {
                            PushPackageIntoDestinationDevice(inter_packet, moduleObject);
                        }
                    }
                }
            }

        }
        // Execute service transfer data from queue in( caculated delay time, preamble code, packet loss, conlision,...) then add to queue out
        private InternalPacket ExecuteTransferDataToQueueOut(int mode, DataProcessed packet, ModuleObject moduleObject)
        {
            if (moduleObject.type == ModuleObject.LORA)
            {
                var parameter = (LoraParameterObject)moduleObject.parameters;
                switch (mode)
                {
                    case NodeDevice.MODE_NORMAL:
                        return new InternalPacket()
                        {
                            packet = packet,
                            DelayTime = CaculateService.caculateDelayTime(parameter.AirRate, packet.data),
                        };
                    case NodeDevice.MODE_WAKEUP:
                        return new InternalPacket()
                        {
                            packet = packet,
                            DelayTime = CaculateService.caculateDelayTime(parameter.AirRate, packet.data),
                            PreambleCode = Helper.generatePreamble(Convert.ToInt32(parameter.WORTime))
                        };
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
        }
        // Push package into destination device
        private void PushPackageIntoDestinationDevice(InternalPacket packet, ModuleObject moduleObject)
        {
            if (moduleObject.type == ModuleObject.LORA)
            {
                var loraParameters = (LoraParameterObject)moduleObject.parameters;
                var destinationAddress = loraParameters.DestinationAddress;
                var destinationChannel = loraParameters.DestinationChannel;
                foreach (var hw in Devices)
                {
                    if (hw.moduleObject.type == ModuleObject.LORA)
                    {
                        if (loraParameters.FixedMode == FixedMode.BROARDCAST) // broadcast
                        {
                            if (hw.moduleObject.parameters is LoraParameterObject)
                            {
                                var hw_loraParameters = (LoraParameterObject)hw.moduleObject.parameters;
                                if (hw_loraParameters.DestinationChannel == destinationChannel)
                                {
                                    // check mode of destination device
                                    if (hw.mode == NodeDevice.MODE_NORMAL)
                                    {
                                        hw.packetQueueOut.Enqueue(packet);
                                    }
                                    else if (hw.mode == NodeDevice.MODE_WAKEUP)
                                    {
                                        // check preamble code
                                        if (packet.PreambleCode != null)
                                        {
                                            hw.packetQueueOut.Enqueue(packet);
                                        }
                                    }
                                }
                            }
                        }
                        else // fixed
                        {
                            if (hw.moduleObject.parameters is LoraParameterObject)
                            {
                                var hw_loraParameters = (LoraParameterObject)hw.moduleObject.parameters;
                                if (hw_loraParameters.DestinationAddress == destinationAddress && hw_loraParameters.DestinationChannel == destinationChannel)
                                {
                                    // check mode of destination device
                                    if (hw.mode == NodeDevice.MODE_NORMAL)
                                    {
                                        hw.packetQueueOut.Enqueue(packet);
                                    }
                                    else if (hw.mode == NodeDevice.MODE_WAKEUP)
                                    {
                                        // check preamble code
                                        if (packet.PreambleCode != null)
                                        {
                                            hw.packetQueueOut.Enqueue(packet);
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
                /*                var zigbeeParameters = (ZigbeeParameterObject)moduleObject.parameters;
                                var destinationAddress = zigbeeParameters.DestinationAddress;
                                var destinationChannel = zigbeeParameters.DestinationChannel;
                                foreach (var hw in Devices)
                                {
                                    if (hw.moduleObject.type == "zigbee")
                                    {
                                        if (zigbeeParameters.FixedMode == "0") // broadcast
                                        {
                                            if (hw.moduleObject.parameters is ZigbeeParameterObject)
                                            {
                                                var hw_zigbeeParameters = (ZigbeeParameterObject)hw.moduleObject.parameters;
                                                if (hw_zigbeeParameters.DestinationChannel == destinationChannel)
                                                {
                                                    hw.packetQueueOut.Enqueue(packet);
                                                }
                                            }
                                        }
                                        else // fixed
                                        {
                                            if (hw.moduleObject.parameters is ZigbeeParameterObject)
                                            {
                                                var hw_zigbeeParameters = (ZigbeeParameterObject)hw.moduleObject.parameters;
                                                if (hw_zigbeeParameters.DestinationAddress == destinationAddress && hw_zigbeeParameters.DestinationChannel == destinationChannel)
                                                {
                                                    hw.packetQueueOut.Enqueue(packet);
                                                }
                                            }
                                        }
                                    }
                                }*/
            }
        }
        // transfer data from queue out to hardware. Delay time is caculated by CaculateService then send to hardware
        private void transferDataToHardware(int mode, SerialPort serialPort, ConcurrentQueue<InternalPacket> packetQueue, ModuleObject moduleObject)
        {
            while (State == RUN)
            {
                if (mode != NodeDevice.MODE_POWERSAVING && mode != NodeDevice.MODE_SLEEP)
                {
                    if (packetQueue.TryDequeue(out InternalPacket packet))
                    {

                        /*create task to delay time and after that send packet to hardware
                         * To do: caculate delay time
                         */

                        // format packet before send, follow protocol
                        PacketTransmit packetTransmit = Helper.formatDataFollowProtocol(PacketTransmit.SENDDATA, packet.packet.data);
                        serialPort.Write(packetTransmit.getPacket(), 0, packetTransmit.getPacket().Length);
                    }
                }
            }
        }

        public void Pause()
        {
            State = IDLE;
            foreach (var hw in Devices)
            {
                hw.serialport.Close();
            }
        }
        public void Stop()
        {
            State = STOP;
            foreach (var hw in Devices)
            {
                hw.readDataFromHardware.Join();
                hw.transferDataIn.Join();
                hw.transferDataOut.Join();
            }
        }

    }
}
