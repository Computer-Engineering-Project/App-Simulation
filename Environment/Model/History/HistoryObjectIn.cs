using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Model.History
{
    public class HistoryObjectIn: BaseModel
    {
        private int id;
        public int Id { get => id; set { id = value; OnPropertyChanged(); } }

        private string source;
        public string Source { get => source; set { source = value; OnPropertyChanged(); } }

        public string data;
        public string Data { get => data; set { data = value; OnPropertyChanged(); } }

        public string delayTime { get; set; }
        public string DelayTime { get => delayTime; set { delayTime = value; OnPropertyChanged(); } }


    }
    public class HistoryObjectOut: BaseModel
    {
        private int id;
        public int Id { get => id; set { id = value; OnPropertyChanged(); } }
        public string data;
        public string Data { get => data; set { data = value; OnPropertyChanged(); } }
    }
}
