using Environment.Model.Module;
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
    public class HistoryObject : BaseModel
    {
        public int id;
        public int Id { get => id; set { id = value; OnPropertyChanged(); } }

        public string time;
        public string Time { get => time; set { time = value; OnPropertyChanged(); } }

        private string source;
        public string Source { get => source; set { source = value; OnPropertyChanged(); } }

        private string data;
        public string Data { get => data; set { data = value; OnPropertyChanged(); } }

        private string delayTime { get; set; }
        public string DelayTime { get => delayTime; set { delayTime = value; OnPropertyChanged(); } }
        private string distance { get; set; }
        public string Distance { get => distance; set { distance = value; OnPropertyChanged(); } }
        private string rssi { get; set; }
        public string RSSI { get => rssi; set { rssi = value; OnPropertyChanged(); } }
        private string snr { get; set; }
        public string SNR { get => snr; set { snr = value; OnPropertyChanged(); } }

        public int typeError;
        public int TypeError { get => typeError; set { typeError = value; OnPropertyChanged(); } }

        public string lossProbality;
        public string LossProbality { get => lossProbality; set { lossProbality = value; OnPropertyChanged(); } }


    }
}
