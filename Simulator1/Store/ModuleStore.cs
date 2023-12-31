﻿using Environment.Model.Module;
using Environment.Model.Packet;
using Environment.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Simulator1.Database;
using Simulator1.State_Management;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Simulator1.Store
{
    public class ModuleStore
    {
        private readonly IServiceProvider serviceProvider;
        private readonly LoadHistoryFile loadHistory;
        private readonly MainStateManagement mainStateManagement;
        private readonly HistoryStateManagement historyStateManagement;

        public List<ModuleObject> ModuleObjects { get; set; }
        public List<string> Ports { get; set; }
        public ModuleStore(IServiceProvider serviceProvider, LoadHistoryFile loadHistory, MainStateManagement mainStateManagement,
            HistoryStateManagement historyStateManagement)
        {
            this.serviceProvider = serviceProvider;
            this.loadHistory = loadHistory;
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
            var type = "";
            if (packet.module == PacketTransmit.LORA)
            {
                type = ModuleObjectType.LORA;
            }
            else if (packet.module == PacketTransmit.ZIGBEE)
            {
                type = ModuleObjectType.ZIGBEE;
            }

            /*foreach (var module in ModuleObjects)
            {
                if (module.port == portName)
                {
                    module.id = id;
                    module.type = type;
                }
            }*/
            if (loadHistory.readConfigFromFile(id, type))
            {
                if (loadHistory.choosenModule == null)
                {
                    return null;
                }
                return loadHistory.choosenModule.parameters;
            }
            /*foreach (var module in loadHistory.listInModules)
            {
                if (module.id == id && module.type == type)
                {
                    return module.parameters;
                }
            }*/
            return null;
        }
        public void Reset()
        {
            ModuleObjects.Clear();
        }
        public bool SaveHistoryToJsonFile(string saveType)
        {
            var saveModuleObjects = ModuleObjects.Select(x => new ModuleObjectDTO()
            {
                id = x.id,
                type = x.type,
                x = x.x,
                y = x.y,
                parameters = x.parameters,
                coveringAreaRange = x.coveringAreaRange
            }).ToList();
            string json = JsonConvert.SerializeObject(saveModuleObjects);
            return loadHistory.SaveJsonFile(json, saveType);
        }
        private void OnLoadHistory()
        {
            try
            {
                if (loadHistory.OpenJsonFile())
                {
                    mainStateManagement.resetAll();
                    ModuleObjects = loadHistory.listInModules;
                    historyStateManagement.loadHistoryModuleFromFile();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
    }
}
