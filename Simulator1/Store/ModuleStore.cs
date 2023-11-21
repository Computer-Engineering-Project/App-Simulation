using Environment.Model.Module;
using Environment.Model.Packet;
using Environment.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Simulator1.Database;
using Simulator1.State_Management;
using System;
using System.Collections.Generic;

namespace Simulator1.Store
{
    public class ModuleStore
    {
        private readonly IServiceProvider serviceProvider;
        private readonly LoadParameter loadParameter;
        private readonly MainStateManagement mainStateManagement;

        public List<ModuleObject> ModuleObjects { get; set; }
        public List<ModuleObject> ModuleObjectDBs { get; set; }
        public List<string> Ports { get; set; }
        public ModuleStore(IServiceProvider serviceProvider, LoadParameter loadParameter, MainStateManagement mainStateManagement)
        {
            this.serviceProvider = serviceProvider;
            this.loadParameter = loadParameter;
            this.mainStateManagement = mainStateManagement;

            this.mainStateManagement.LoadHistory += OnLoadHistory;

            ModuleObjects = new List<ModuleObject>();
            ModuleObjectDBs= new List<ModuleObject>(loadParameter.listInModules);
            Ports= new List<string>();
            /*var enviromentService = serviceProvider.GetRequiredService<IEnvironmentService>();
            Ports = new List<string>(enviromentService.getPorts());*/
        }
        public object LoadParametersFromHardware(string portName)
        {
            PacketTransmit packet = serviceProvider.GetRequiredService<IEnvironmentService>().getIdTypeFromHardware(portName);
            var id = packet.data[0].ToString();
            var type = packet.module.ToString();
            /*foreach (var module in ModuleObjects)
            {
                if (module.port == portName)
                {
                    module.id = id;
                    module.type = type;
                }
            }*/
            foreach (var module in ModuleObjectDBs)
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
            ModuleObjects = ModuleObjectDBs;
        }
    }
}
