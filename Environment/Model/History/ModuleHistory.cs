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
        public ConcurrentQueue<HistoryObjectIn> historyObjectIns= new ConcurrentQueue<HistoryObjectIn>();
        public ConcurrentQueue<HistoryObjectOut> historyObjectOuts= new ConcurrentQueue<HistoryObjectOut>();
    }
}
