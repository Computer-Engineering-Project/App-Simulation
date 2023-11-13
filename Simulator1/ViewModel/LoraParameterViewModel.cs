using Simulator1.Model;
using Simulator1.State_Management;
using Simulator1.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Simulator1.ViewModel
{
    public class LoraParameterViewModel : BaseViewModel
    {
        private string address;
        public string Address { get => address; set { address = value; OnPropertyChanged(); } }

        private string channel;
        public string Channel { get => channel; set { channel = value; OnPropertyChanged(); } }

        private string airRate;
        public string AirRate { get => airRate; set { airRate = value; OnPropertyChanged(); } }

        private string powerTransmit;
        public string PowerTransmit { get => powerTransmit; set { powerTransmit = value; OnPropertyChanged(); } }

        private string uartRate;
        public string UartRate { get => uartRate; set { uartRate = value; OnPropertyChanged(); } }

        private string fixedMode;
        public string FixedMode { get => fixedMode; set { fixedMode = value; OnPropertyChanged(); } }

        private string worTime;
        public string WORTime { get => worTime; set { worTime = value; OnPropertyChanged(); } }

        private string parity;
        public string Parity { get => parity; set { parity = value; OnPropertyChanged(); } }

        private string ioMode;
        public string IOMode { get => ioMode; set { ioMode = value; OnPropertyChanged(); } }

        private string fec;
        public string FEC { get => fec; set { fec = value; OnPropertyChanged(); } }


        private readonly ModuleStateManagement moduleStateManagement;
        private readonly ModuleStore moduleStore;

        public LoraParameterViewModel(ModuleStateManagement moduleStateManagement, ModuleStore moduleStore)
        {
            this.moduleStateManagement = moduleStateManagement;
            this.moduleStore = moduleStore;

            this.moduleStateManagement.LoraParamsCreated += OnCreateLoraParameter;
        }

        private LoraParameterObject createLoraParamsObject()
        {
            return new LoraParameterObject()
            {
                UartRate = UartRate,
                Address = Address,
                Channel = Channel,
                AirRate = AirRate,
                Power = PowerTransmit,
                FixedMode = FixedMode,
                WORTime = WORTime,
                Parity = Parity,
                IOMode = IOMode,
                FEC = FEC
            };
        }
        private void OnCreateLoraParameter(ModuleObject module)
        {
            var loraParams = createLoraParamsObject();
            module.parameters = loraParams;
            module.type = "Lora";
            moduleStore.ModuleObjects.Add(module);
            MessageBox.Show("create object success!");
        }
    }
}
