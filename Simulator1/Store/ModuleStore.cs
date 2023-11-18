using Environment.Model.Module;
using Environment.Model.Packet;
using Environment.Service.Interface;
using Simulator1.Database;
using Simulator1.State_Management;
using System.Collections.Generic;

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
            PacketTransmit packet = environmentService.getIdTypeFromHardware(portName);
            var id = packet.data[0].ToString();
            var type = packet.module.ToString();

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
