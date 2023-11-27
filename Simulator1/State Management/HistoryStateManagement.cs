using Environment.Model.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator1.State_Management
{
    public class HistoryStateManagement
    {
        public event Action<ModuleObject> ReloadHistoryData;
        public event Action LoadHistoryModuleFromFile;
        public void reloadHistoryData(ModuleObject moduleObject)
        {
            ReloadHistoryData?.Invoke(moduleObject);
        }
        public void loadHistoryModuleFromFile()
        {
            LoadHistoryModuleFromFile?.Invoke();
        }
    }
}
