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
            EnvState.ModeModule = MODE_MODULE.CONFIG;
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
            EnvState.PreProgramStatus = EnvState.ProgramStatus;
            EnvState.ProgramStatus = PROGRAM_STATUS.RUN;
            EnvState.ModeModule = MODE_MODULE.SEND_DATA;
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
            EnvState.PreProgramStatus = EnvState.ProgramStatus;
            EnvState.ProgramStatus = PROGRAM_STATUS.IDLE;
        }

        public void passPortClicked(string portName)
        {
            environment.portClicked = portName;
        }

        public void Pause()
        {
            EnvState.PreProgramStatus = EnvState.ProgramStatus;
            EnvState.ProgramStatus = PROGRAM_STATUS.PAUSE;
        }

        public void changeModuleObjectsPosition(List<ModuleObject> moduleObjects)
        {
            environment.ModuleObjects = moduleObjects;
            environment.ResetParamsForDevice();
        }

        public void closeThreads()
        {
            /* foreach(var hw in environment.Devices)
             {
                 hw.readDataFromHardware.Join();
                 hw.transferDataIn.Join();
                 hw.transferDataOut.Join();
                 environment.Devices.Remove(hw);
             }*/
        }
        public void setNoise(string noise)
        {
            environment.Noise = Double.Parse(noise);
        }
    }
}
