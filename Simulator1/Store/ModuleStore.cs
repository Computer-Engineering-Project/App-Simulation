using Environment.Model.History;
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
        private readonly LoadHistoryFile loadParameter;
        private readonly MainStateManagement mainStateManagement;
        private readonly HistoryStateManagement historyStateManagement;

        public List<ModuleObject> ModuleObjects { get; set; }
        public List<string> Ports { get; set; }
        public ModuleStore(IServiceProvider serviceProvider, LoadHistoryFile loadParameter, MainStateManagement mainStateManagement, 
            HistoryStateManagement historyStateManagement)
        {
            this.serviceProvider = serviceProvider;
            this.loadParameter = loadParameter;
            this.mainStateManagement = mainStateManagement;
            this.historyStateManagement = historyStateManagement;



            this.mainStateManagement.LoadHistory += OnLoadHistory;

            ModuleObjects = new List<ModuleObject>();
            Ports = new List<string>();
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
            foreach (var module in loadParameter.listInModules)
            {
                if (module.id == id && module.type == type)
                {
                    return module.parameters;
                }
            }
            return null;
        }
        public void Reset()
        {
            ModuleObjects.Clear();
        }
        private void OnLoadHistory()
        {
            if (loadParameter.ScanJsonFile())
            {
                mainStateManagement.resetAll();
                ModuleObjects = loadParameter.listInModules;
                historyStateManagement.loadHistoryModuleFromFile();
            }
        }
    }
}
