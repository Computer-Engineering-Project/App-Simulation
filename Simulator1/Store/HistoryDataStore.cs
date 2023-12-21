using Environment.Model.History;
using Environment.Model.Module;
using Simulator1.State_Management;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator1.Store
{
    public class HistoryDataStore
    {
        private readonly ModuleStore moduleStore;
        private readonly HistoryStateManagement historyStateManagement;
        private readonly MainStateManagement mainStateManagement;
        public List<ModuleHistory> ModuleHistories;
        public HistoryDataStore(ModuleStore moduleStore, HistoryStateManagement historyStateManagement, MainStateManagement mainStateManagement)
        {
            this.moduleStore = moduleStore;
            this.historyStateManagement = historyStateManagement;
            this.mainStateManagement = mainStateManagement;

            this.historyStateManagement.ReloadHistoryData += OnReloadHistoryData;
            this.historyStateManagement.LoadHistoryModuleFromFile += OnLoadHistoryModuleFromFile;
            ModuleHistories = new List<ModuleHistory>();
        }
        public void OnReloadHistoryData(ModuleObject moduleObject)
        {
            var moduleHistory = ModuleHistories.FirstOrDefault(x => x.moduleObject.id == moduleObject.id);
            if (moduleHistory != null)
            {
                moduleHistory.moduleObject = moduleObject;
            }
        }
        public void OnLoadHistoryModuleFromFile()
        {
            
            foreach(var m_object in moduleStore.ModuleObjects)
            {
                ModuleHistories.Clear();
                ModuleHistories.Add(new ModuleHistory()
                {
                    moduleObject = m_object
                });
            }
        }
        public void ClearHistoryData()
        {
            foreach(var history in ModuleHistories)
            {
                history.UI_historyObjectOuts.Clear();
                history.UI_historyObjectIns.Clear();
                history.UI_historyObjectErrors.Clear();
            }
        }
    }
}
