using Environment.Model;
using Environment.Model.Packet;
using Environment.Service.Interface;
using System;
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
        private readonly ICommunicationService communicationService;

        public BaseEnvironment(ICommunicationService communicationService)
        {
            this.communicationService = communicationService;
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
            byte[] data = { 0x00 };
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
                    var objectIn = new NodeDeviceIn()
                    {
                        serialport = serialport
                    };
                    DeviceGoOut.Add(objectIn);
                }
            }
            foreach (var packet in DeviceGoOut)
            {
                packet.serialport.DataReceived += new SerialDataReceivedEventHandler(addToQueue);
            }
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

                    foreach (var hardware in DeviceGoOut)
                    {
                        if (hardware.serialport.PortName == ((SerialPort)sender).PortName)
                        {
                            hardware.packetQueue.Enqueue(dataProcessed);
                        }
                    }
                }
            }
        }
        public void Run()
        {
            createSerialPortInitial();
        }
    }
}
