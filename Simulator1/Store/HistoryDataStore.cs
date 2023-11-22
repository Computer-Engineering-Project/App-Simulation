using Environment.Model.History;
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
        public List<ModuleHistory> ModuleHistories; 
        public HistoryDataStore()
        {
            ModuleHistories= new List<ModuleHistory>();
        }
        public void ClearHistoryData()
        {
            ModuleHistories.Clear();
        }
    }
}
