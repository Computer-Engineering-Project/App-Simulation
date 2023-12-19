using Environment.Model.Module;
using Environment.Model.VMParameter;
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
        public event Action<ModuleObject> UpdateLoraParamsOfModule;
        public event Action<LoraParameterObject> OpenUpdateLoraParams;
        public event Action<Dictionary<string, string>> ReadLoraConfigParams;

        public event Action<ModuleObject> ZigbeeParamsCreated;
        public event Action<ModuleObject> UpdateZigbeeParamsOfModule;
        public event Action<ZigbeeParameterObject> OpenUpdateZigbeeParams;
        public event Action<Dictionary<string, string>> ReadZigbeeConfigParams;

        public event Action<string> ConfigParams;
        public event Action<object> UpdatePosition;
        public event Action ResetParameterModule;
        public event Action<ModuleParameterVM> UpdateParamsForModuleParameterVM;

        public event Action<object> IsActionUpdate;
        public event Action<object> ChangePositionAndPort;
        public event Action<ModuleObject> ConfigHardwareSuccess;
        public event Action<string> DeleteModule;
        public void deleteModule(string portName)
        {
            DeleteModule?.Invoke(portName);
        }
        public void createModuleObject(string portName)
        {
            ModuleObjectCreated?.Invoke(portName);
        }
        public void createLoraParameter(ModuleObject module)
        {
            LoraParamsCreated?.Invoke(module);
        }
        public void createZigbeeParameter(ModuleObject module)
        {
            ZigbeeParamsCreated?.Invoke(module);
        }
        public void openUpdateLoraParameter(LoraParameterObject loraParameter)
        {
            OpenUpdateLoraParams?.Invoke(loraParameter);
        }
        public void openUpdateZigbeeParameter(ZigbeeParameterObject loraParameter)
        {
            OpenUpdateZigbeeParams?.Invoke(loraParameter);
        }
        public void updateLoraParamsOfModule(ModuleObject module)
        {
            UpdateLoraParamsOfModule?.Invoke(module);
        }
        public void updateZigbeeParamsOfModule(ModuleObject module)
        {
            UpdateZigbeeParamsOfModule?.Invoke(module);
        }

        public void readLoraConfigParameter(Dictionary<string, string> listParams)
        {
            ReadLoraConfigParams?.Invoke(listParams);
        }
        public void readZigbeeConfigParameter(Dictionary<string, string> listParams)
        {
            ReadZigbeeConfigParams?.Invoke(listParams);
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
        public void configHardwareSuccess(ModuleObject moduleObject)
        {
            ConfigHardwareSuccess?.Invoke(moduleObject);
        }
        public void updateModuleVMParams(ModuleParameterVM moduleParameterVM)
        {
            UpdateParamsForModuleParameterVM?.Invoke(moduleParameterVM);
        }
    }
}
