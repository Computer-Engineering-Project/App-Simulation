using Environment.Base;
using Environment.Model.Module;
using Environment.Model.Packet;
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
        private readonly BaseEnvironment environment;
        public EnvironmentService(ICommunication communication)
        {
            environment = new BaseEnvironment(communication); 
        }

        public void ActiveHardware(string port)
        {
            environment.ActiveHardwareDevice(port);
        }

        public PacketTransmit getIdTypeFromHardware(string portName)
        {
            return environment.ExecuteReadConfigFromHardware(portName);
        }

        public void startPort(string portName)
        {
            environment.StartPort(portName);
        }
        public void closePort(string portName)
        {
            environment.ClosePort(portName);
        }

        public void passModuleObjects(List<ModuleObject> moduleObjects)
        {
            environment.ModuleObjects = moduleObjects;
        }

        public void Run()
        {
            environment.Run();
        }
        public List<string> loadPorts()
        {
            environment.SetUp();
            return environment.Ports;
        }
    }
}
