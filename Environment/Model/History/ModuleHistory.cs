using Environment.Model.Module;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Model.History
{
    public class ModuleHistory
    {
        public ModuleObject moduleObject { get; set; }
        public ConcurrentQueue<HistoryObject> UI_historyObjectIns= new ConcurrentQueue<HistoryObject>();
        public ConcurrentQueue<HistoryObject> UI_historyObjectOuts= new ConcurrentQueue<HistoryObject>();
        public ConcurrentQueue<HistoryObject> UI_historyObjectErrors = new ConcurrentQueue<HistoryObject>();
        public ConcurrentQueue<HistoryObject> historyObjectIns = new ConcurrentQueue<HistoryObject>();
        public ConcurrentQueue<HistoryObject> historyObjectOuts = new ConcurrentQueue<HistoryObject>();
        public ConcurrentQueue<HistoryObject> historyObjectErrors = new ConcurrentQueue<HistoryObject>();
    }
}
