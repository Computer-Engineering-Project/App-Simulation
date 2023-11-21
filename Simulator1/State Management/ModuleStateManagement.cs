using Environment.Model.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator1.State_Management
{
    public class ModuleStateManagement
    {
        public event Action<string> ModuleObjectCreated;
        public event Action<ModuleObject> LoraParamsCreated;
        public event Action<ModuleObject> UpdateParamsOfModule;
        public event Action<LoraParameterObject> OpenUpdateLoraParams;
        public event Action<Dictionary<string, string>> ReadLoraConfigParams;
        public event Action<string> ConfigParams;
        public event Action<object> UpdatePosition;
        public event Action ResetParameterModule;

        public event Action<object> IsActionUpdate;
        public event Action<object> ChangePositionAndPort;
        public event Action<string> ConfigHardwareSuccess;
        public void createModuleObject(string portName)
        {
            ModuleObjectCreated?.Invoke(portName);
        }
        public void createLoraParameter(ModuleObject module)
        {
            LoraParamsCreated?.Invoke(module);
        }
        public void openUpdateLoraParameter(LoraParameterObject loraParameter)
        {
            OpenUpdateLoraParams?.Invoke(loraParameter);
        }
        public void updateParamsOfModule(ModuleObject module)
        {
            UpdateParamsOfModule?.Invoke(module);
        }

        public void readLoraConfigParameter(Dictionary<string, string> listParams)
        {
            ReadLoraConfigParams?.Invoke(listParams);
        }
        public void configParameter(string moduleType)
        {
            ConfigParams?.Invoke(moduleType);
        }
        public void updatePosition(object moduleParams)
        {
            UpdatePosition?.Invoke(moduleParams);
        }

        public void resetParameterModule() 
        { 
            ResetParameterModule?.Invoke();
        } 

        public void isActionUpdate(object isUpdated)
        {
            IsActionUpdate?.Invoke(isUpdated);
        }

        public void changePositionAndPort(object moduleParams)
        {
            ChangePositionAndPort?.Invoke(moduleParams);
        }
        public void configHardwareSuccess(string portName)
        {
            ConfigHardwareSuccess?.Invoke(portName);
        }
    }
}
