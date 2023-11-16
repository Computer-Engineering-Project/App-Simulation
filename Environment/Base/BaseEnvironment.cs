using Environment.Model;
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
        public List<ObjectGoIn> HardwareGoIn = new List<ObjectGoIn>();
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
            var serialport = SerialPorts.FirstOrDefault(s => s.PortName == port);
            if (serialport != null)
            {
                return Helper.getConfigFromHardware(serialport);

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
                    var objectIn = new ObjectGoIn()
                    {
                        serialport = serialport,
                    };
                    HardwareGoIn.Add(objectIn);
                }
            }
            foreach (var packet in HardwareGoIn)
            {
                packet.serialport.DataReceived += new SerialDataReceivedEventHandler(addToQueue);
            }
        }
        private void addToQueue(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer;
            var numOfBytes = ((SerialPort)sender).Read(buffer, 0, 58);
        }
        public void Run()
        {
            createSerialPortInitial();
        }
    }
}
