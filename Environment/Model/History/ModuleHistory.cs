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
        public string id { get; set; }
        public string type { get; set; }
        public string portName { get; set; }
        public ConcurrentQueue<HistoryObjectIn> historyObjectIns= new ConcurrentQueue<HistoryObjectIn>();
        public ConcurrentQueue<HistoryObjectOut> historyObjectOuts= new ConcurrentQueue<HistoryObjectOut>();
    }
}
