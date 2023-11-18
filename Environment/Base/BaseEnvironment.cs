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
        public List<NodeDeviceIn> DeviceGoOut = new List<NodeDeviceIn>();
        public List<NodeDeviceOut> GoInDevice = new List<NodeDeviceOut>();
        public List<ModuleObject> ModuleObjects = new List<ModuleObject>();

        Thread transferDataToView;
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
                var stringActive = Helper.createStringActiveHardware();
                if (stringActive != null)
                {
                    serialport.Write(stringActive, 0, stringActive.Length);
                }
                return;
            }
        }
        public string ExecuteReadConfigFromHardware(string port)
        {
            //return id : string + module type: string
            var type = ModuleObjects.FirstOrDefault(x => x.port == port)?.type;
            foreach (var serialport in SerialPorts)
            {
                if (serialport.PortName == port)
                {
                    /*var stringActive = Helper.createStringReadConfigHardware(); // tạo hàm này trong helper hoặc chỗ nào tùy mi 
                    if (stringActive != null)
                    {
                        serialport.Write(stringActive, 0, stringActive.Length);
                    }
                    byte[] buffer = new byte[58];
                    var numofByte = 0;
                    while (numofByte == 0)
                    {
                        numofByte = serialport.Read(buffer, 0, buffer.Length);
                    }
                    // execute the buffer,
                    return "";*/
                }
            }
            return "001:E32";
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
                    var objectIn = new NodeDeviceIn()
                    {
                        serialport = serialport
                    };
                    var objectOut = new NodeDeviceOut()
                    {
                        serialport = serialport
                    };
                    DeviceGoOut.Add(objectIn);
                    GoInDevice.Add(objectOut);
                }
            }
            foreach (var packet in DeviceGoOut)
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
            byte[] buffer = new byte[58];
            var numOfBytes = ((SerialPort)sender).Read(buffer, 0, 58);
            if (numOfBytes > 0)
            {
                //execute buffer here
                foreach (var hw in DeviceGoOut)
                {
                    if (hw.serialport.PortName == ((SerialPort)sender).PortName && hw.mode != "2" && hw.mode != "3")
                    {
                        hw.packetQueue.Enqueue(new PacketTransmit()
                        {
                            
                        }) ;
                        return;
                    }
                }
            }
        }
        //Run program =====
        public void ChangeMode(string portName, string mode)
        {
            foreach (var hw in DeviceGoOut)
            {
                if (hw.serialport.PortName == portName)
                {
                    lock (hw.lockObject)
                    {
                        hw.mode = mode;
                    }
                }
            }
        }
        public void Run()
        {
            createSerialPortInitial();
            /*transferDataToView = new Thread(transferInPacketToView);
            transferDataToView.Start();*/
            foreach (var hw in DeviceGoOut)
            {
                var moduleObject = ModuleObjects.FirstOrDefault(x => x.port == hw.serialport.PortName);
                if (moduleObject != null)
                {
                    hw.transferData = new Thread(() => transferDataToAvailableDevice(hw.mode, hw.serialport, hw.packetQueue, moduleObject));
                    hw.transferData.Start();
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
        private void transferDataToAvailableDevice(string mode, SerialPort serialPort, ConcurrentQueue<PacketTransmit> packetQueue, ModuleObject moduleObject)
        {
            if (mode != "2" && mode != "3")
            {
                if (packetQueue.TryDequeue(out PacketTransmit packet))
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
        private InternalPacket ExecuteTransferData(string mode, PacketTransmit packet, ModuleObject moduleObject)
        {
            if (moduleObject.type == "lora")
            {
                var parameter = (LoraParameterObject)moduleObject.parameters;
                switch (mode)
                {
                    case "0":
                        return new InternalPacket()
                        {
                            packet = packet,
                            DelayTime = Helper.caculateDelayTime(parameter.AirRate, packet.dataLength),
                        };
                    case "1":
                        return new InternalPacket()
                        {
                            packet = packet,
                            DelayTime = Helper.caculateDelayTime(parameter.AirRate, packet.dataLength),
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
                
            }
        }
    }
}
