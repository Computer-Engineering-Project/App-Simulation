using Environment.Model;
using Environment.Model.Module;
using Environment.Model.Packet;
using Environment.Service.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Base
{
    public class BaseEnvironment
    {
        public List<string> Ports = new List<string>();
        public List<SerialPort> SerialPorts = new List<SerialPort>();
        public List<NodeDevice> Devices = new List<NodeDevice>();
        public List<ModuleObject> ModuleObjects = new List<ModuleObject>();

        private readonly ICommunication communication;

        public BaseEnvironment(ICommunication communication)
        {
            this.communication = communication;
            SetUp();
        }

        private void SetUp()
        {
            var ports = SerialPort.GetPortNames();
            Ports.AddRange(ports);
        }
        public void StartPort(string port)
        {
            var serialPort = new SerialPort(port, 115200);
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
            }
            SerialPorts.Add(serialPort);
        }
        public void ActiveHardwareDevice(string port)
        {
            var serialport = SerialPorts.FirstOrDefault(e => e.PortName == port);
            if (serialport != null)
            {
                byte [] stringActive = Helper.CmdActiveHardware();
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

        public bool ExecuteConfigFromHardware(string port)
        {
            //return id : string + module type: string
            var serialport = SerialPorts.FirstOrDefault(s => s.PortName == port);
            byte module = 0x01;
            // data is id module
            byte[] data = { 0x00, 0x07 }; // id = 0, baudrate = 0x07
            if (serialport != null)
            {
                return Helper.SendCmdConfigToHardware(serialport, module, data);

            }
            return false;
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
        private void createSerialPortInitial()
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
            foreach (var packet in Devices)
            {
                packet.serialport.DataReceived += new SerialDataReceivedEventHandler(addToQueue);
            }
        }
        //Function listen from hardware
        private string listenConfigFromHardware(SerialPort serialPort)
        {
            byte[] buffer = new byte[58];
            var numOfBytes = serialPort.Read(buffer, 0, 58);
            if (numOfBytes > 0)
            {
                //execute buffer here
                return "";
            }
            return "";
        }
        private void addToQueue(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = Helper.GetDataFromHardware((SerialPort)sender);
            if (buffer.Length > 0)
            {
                var packet = Helper.HandleMessFromHardware(buffer);
                if (packet != null)
                {
                    DataProcessed dataProcessed = new DataProcessed(packet.data);

                    foreach (var hardware in Devices)
                    {
                        if (hardware.serialport.PortName == ((SerialPort)sender).PortName)
                        {
                            hardware.packetQueueIn.Enqueue(dataProcessed);
                        }
                    }
                }
            }
        }
        //Run program =====
        public void Run()
        {
            createSerialPortInitial();
            /*transferDataToView = new Thread(transferInPacketToView);
            transferDataToView.Start();*/
            foreach (var hw in Devices)
            {
                var moduleObject = ModuleObjects.FirstOrDefault(x => x.port == hw.serialport.PortName);
                if (moduleObject != null)
                {
                    hw.transferDataIn = new Thread(() => transferDataToAvailableDevice(hw.mode, hw.serialport, hw.packetQueueIn, moduleObject));
                    hw.transferDataIn.Start();
                }
            }
        }
        /*private void transferInPacketToView()
        {
            var listTransferedPacket = new List<PacketTransferToView>();
            foreach (var hw in HardwareGoIn)
            {
                var cpy_queue = hw.packetQueue;
                if (cpy_queue.TryDequeue(out PacketTransmit packet))
                {
                    listTransferedPacket.Add(new PacketTransferToView()
                    {
                        portName = hw.serialport.PortName,
                        packet = packet,
                    });

                }
            }
            
        }*/
        private void transferDataToAvailableDevice(int mode, SerialPort serialPort, ConcurrentQueue<DataProcessed> packetQueue, ModuleObject moduleObject)
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
                    var inter_packet = ExecuteTransferData(mode, packet, moduleObject);
                    if (inter_packet != null)
                    {
                        PushPackageIntoAvailableDevice(inter_packet, moduleObject);
                    }
                }
            }
        }
        private InternalPacket ExecuteTransferData(int mode, DataProcessed packet, ModuleObject moduleObject)
        {
            if (moduleObject.type == "lora")
            {
                var parameter = (LoraParameterObject)moduleObject.parameters;
                switch (mode)
                {
                    case NodeDevice.MODE_NORMAL:
                        return new InternalPacket()
                        {
                            packet = packet,
                            DelayTime = Helper.caculateDelayTime(parameter.AirRate, packet.data),
                        };
                    case NodeDevice.MODE_WAKEUP:
                        return new InternalPacket()
                        {
                            packet = packet,
                            DelayTime = Helper.caculateDelayTime(parameter.AirRate, packet.data),
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
        private void PushPackageIntoAvailableDevice(InternalPacket packet, ModuleObject moduleObject)
        {
            if (moduleObject.type == "lora")
            {
                var loraParameters = (LoraParameterObject)moduleObject.parameters;
                var destinationAddress = loraParameters.DestinationAddress;
                var destinationChannel = loraParameters.DestinationChannel;
                foreach(var hw in Devices)
                {
                    if (hw.moduleObject.type == "lora")
                    {
                        if(loraParameters.FixedMode == "0") // broadcast
                        {
                            if (hw.moduleObject.parameters is LoraParameterObject)
                            {
                                var hw_loraParameters = (LoraParameterObject)hw.moduleObject.parameters;
                                if (hw_loraParameters.DestinationChannel == destinationChannel)
                                {
                                    hw.packetQueueOut.Enqueue(packet);
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
                                    hw.packetQueueOut.Enqueue(packet);
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


    }
}
