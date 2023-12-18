using Simulator1.State_Management;
using Simulator1.Store;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator1.ViewModel
{
    public class ZigbeeParameterViewModel : BaseViewModel
    {
        private ObservableCollection<string> listUartRate;
        public ObservableCollection<string> ListUartRate { get => listUartRate; set { listUartRate = value; OnPropertyChanged(); } }

        private ObservableCollection<string> listFixedMode;
        public ObservableCollection<string> ListFixedMode { get => listFixedMode; set { listFixedMode = value; OnPropertyChanged(); } }

        private string id;
        public string Id { get => id; set { id = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private string channel;
        public string Channel { get => channel; set { channel = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private string airRate;
        public string AirRate { get => airRate; set { airRate = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private string powerTransmit;
        public string PowerTransmit { get => powerTransmit; set { powerTransmit = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private string uartRate;
        public string UartRate { get => uartRate; set { uartRate = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private string fixedMode;
        public string FixedMode { get => fixedMode; set { fixedMode = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private string antennaGain;
        public string AntennaGain { get => antennaGain; set { antennaGain = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }




        private readonly ModuleStateManagement moduleStateManagement;
        private readonly ModuleStore moduleStore;
        private readonly IServiceProvider serviceProvider;
        private readonly StatusStateManagement statusStateManagement;

        public ZigbeeParameterViewModel(ModuleStateManagement moduleStateManagement, ModuleStore moduleStore, IServiceProvider serviceProvider,
            StatusStateManagement statusStateManagement)
        {
            this.moduleStateManagement = moduleStateManagement;
            this.moduleStore = moduleStore;
            this.serviceProvider = serviceProvider;
            this.statusStateManagement = statusStateManagement;


            /*            this.moduleStateManagement.ConfigParams += OnConfigParameterToHardware;*/

            ListUartRate = new ObservableCollection<string>() { "2400", "4800", "9600", "19200", "38400", "57600", "115200" };

            ListFixedMode = new ObservableCollection<string>() { "Point-To-Point", "Broadcast" };

            //Event Delegate

        }
    }
}
