using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Base
{
    public class BaseEnvironment
    {
        public List<string> Ports = new List<string>();
        public List<SerialPort> SerialPorts = new List<SerialPort>();
        public BaseEnvironment()
        {
            SetUp();
            /*            StartPort();*/
        }

        private void SetUp()
        {
            var ports = SerialPort.GetPortNames();
            foreach (var port in ports)
            {
                Ports.Add(port);
            }
        }
        public void StartPort(string port)
        {
            var serialPort = new SerialPort(port, 115200);
            SerialPorts.Add(serialPort);
        }

        public void ActiveHardwareDevice(string port)
        {
            foreach (var serialport in SerialPorts)
            {
                if (serialport.PortName == port)
                {
                    var stringActive = Helper.createStringActiveHardware();
                    if (stringActive != null)
                    {
                        serialport.Write(stringActive, 0, stringActive.Length);
                    }
                }
            }

        }
        public string ExecuteReadConfigFromHardware(string port)
        {
            //return id : string + module type: string
            foreach(var serialport in SerialPorts)
            {
                if(serialport.PortName == port)
                {
                    //Get Id, type module by Helper function
                }
            }
            return "";
        }
        public void ExecuteConfigDevice(string port)
        {

        }

    }
}
