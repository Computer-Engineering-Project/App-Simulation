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
        public void createModuleObject()
        {
            ModuleObjectCreated?.Invoke();
        }
        public void createLoraParameter(ModuleObject module)
        {
            LoraParamsCreated?.Invoke(module);
        }
    }
}
