using Simulator1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator1.State_Management
{
    public class ModuleStateManagement
    {
        public event Action ModuleObjectCreated;
        public event Action<ModuleObject> LoraParamsCreated;
        public event Action<LoraParameterObject> UpdateLoraParams;
        public event Action<Dictionary<string, string>> ReadLoraConfigParams;
        public event Action<Object> UpdatePositionAndPort;
        public event Action ResetParameterModule;
        public void createModuleObject()
        {
            ModuleObjectCreated?.Invoke();
        }
        public void createLoraParameter(ModuleObject module)
        {
            LoraParamsCreated?.Invoke(module);
        }
        public void updateLoraParameter(LoraParameterObject loraParameter)
        {
            UpdateLoraParams?.Invoke(loraParameter);
        }
        public void readLoraConfigParameter(Dictionary<string, string> listParams)
        {
            ReadLoraConfigParams?.Invoke(listParams);
        }

        public void updatePositionAndPort(Object moduleParams)
        {
            UpdatePositionAndPort?.Invoke(moduleParams);
        }

        public void resetParameterModule() 
        { 
            ResetParameterModule?.Invoke();
        } 
    }
}
