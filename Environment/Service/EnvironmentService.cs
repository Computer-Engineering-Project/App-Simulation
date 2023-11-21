using Environment.Base;
using Environment.Model.Module;
using Environment.Model.Packet;
using Environment.Service.Interface;
using Newtonsoft.Json;
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
        public bool configHardware(string portName, object parameters)
        {
            string json = JsonConvert.SerializeObject(parameters);
            Dictionary<string, string> listParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (listParams != null)
            {
                return environment.ExecuteConfigToHardware(portName, listParams["module"], listParams["id"], listParams["baudrate"]);
            }
            return false;
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
            environment.State = environment.RUN;
            environment.createSerialPortInitial();
            environment.RunProgram();
           
        }
        public List<string> loadPorts()
        {
            environment.SetUp();
            return environment.Ports;
        }

        public void Stop()
        {
            environment.State = environment.STOP;
        }
    }
}
