using Environment.Base;
using Environment.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Service
{
    public class EnvironmentService : IEnvironmentService
    {
        private readonly ICommunicationService communicationService;
        private readonly BaseEnvironment environment;
        public EnvironmentService(ICommunicationService communicationService)
        {
            this.communicationService = communicationService;
            environment = new BaseEnvironment(communicationService); 
        }

        public void ActiveHardware(string port)
        {
            environment.ActiveHardwareDevice(port);
        }

        public string getIdTypeFromHardware(string portName)
        {
            return environment.ExecuteReadConfigFromHardware(portName);
        }

        public List<string> getPorts()
        {
            return environment.Ports;
        }

        public void startPort(string portName)
        {
            environment.StartPort(portName);
        }
        public void closePort(string portName)
        {
            environment.ClosePort(portName);
        }
    }
}
