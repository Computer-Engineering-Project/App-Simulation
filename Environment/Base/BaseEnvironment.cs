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
        public BaseEnvironment()
        {
            SetUp();
        }

        private void SetUp()
        {
            var ports = SerialPort.GetPortNames();
            foreach(var port in ports)
            {
                Ports.Add(port);
            }
        }
    }
}
