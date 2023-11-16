using Environment.Service;
using Environment.Service.Interface;
using Simulator1.Database;
using Simulator1.Model;
using Simulator1.State_Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator1.Store
{
    public class ModuleStore
    {
        private readonly IEnvironmentService environmentService;
        private readonly LoadParameter loadParameter;
        private readonly MainStateManagement mainStateManagement;

        public List<ModuleObject> ModuleObjects { get; set; }
        public List<string> Ports { get; set; }
        public ModuleStore(IEnvironmentService environmentService, LoadParameter loadParameter, MainStateManagement mainStateManagement)
        {
            this.environmentService = environmentService;
            this.loadParameter = loadParameter;
            this.mainStateManagement = mainStateManagement;

            this.mainStateManagement.LoadHistory += OnLoadHistory;

            ModuleObjects = new List<ModuleObject>();
            Ports = new List<string>(environmentService.getPorts());
        }
        public object LoadParametersFromHardware(string portName)
        {
            var id_type = environmentService.getIdTypeFromHardware(portName).Split(":");
            var id = id_type[0];
            var type = id_type[1];

            var listParams = loadParameter.listInModules;
            foreach ( var module in listParams )
            {
                if (module.id == id && module.type == type)
                {
                    return module.parameters;
                }
            }
            return null;
        }
        private void OnLoadHistory()
        {
            ModuleObjects = loadParameter.listInModules;
        }
    }
}
