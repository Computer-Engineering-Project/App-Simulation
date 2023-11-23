
using Environment.Model.Module;
using Environment.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Simulator1.State_Management;
using Simulator1.Store;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Simulator1.ViewModel
{
    public class LoraParameterViewModel : BaseViewModel
    {

        private ObservableCollection<string> listPower;
        public ObservableCollection<string> ListPower { get => listPower; set { listPower = value; OnPropertyChanged(); } }

        private ObservableCollection<string> listAirRate;
        public ObservableCollection<string> ListAirRate { get => listAirRate; set { listAirRate = value; OnPropertyChanged(); } }

        private ObservableCollection<string> listUartRate;
        public ObservableCollection<string> ListUartRate { get => listUartRate; set { listUartRate = value; OnPropertyChanged(); } }

        private ObservableCollection<string> listWORTime;
        public ObservableCollection<string> ListWORTime { get => listWORTime; set { listWORTime = value; OnPropertyChanged(); } }

        private ObservableCollection<string> listFixedMode;
        public ObservableCollection<string> ListFixedMode { get => listFixedMode; set { listFixedMode = value; OnPropertyChanged(); } }

        private ObservableCollection<string> listParity;
        public ObservableCollection<string> ListParity { get => listParity; set { listParity = value; OnPropertyChanged(); } }

        private ObservableCollection<string> listIOMode;
        public ObservableCollection<string> ListIOMode { get => listIOMode; set { listIOMode = value; OnPropertyChanged(); } }

        private ObservableCollection<string> listFEC;
        public ObservableCollection<string> ListFEC { get => listFEC; set { listFEC = value; OnPropertyChanged(); } }

        private string id;
        public string Id { get => id; set { id = value; OnPropertyChanged(); } }

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

        private string destinationAddress;
        public string DestinationAddress { get => destinationAddress; set { destinationAddress = value; OnPropertyChanged(); } }

        private string destinationChannel;
        public string DestinationChannel { get => destinationChannel; set { destinationChannel = value; OnPropertyChanged(); } }



        private readonly ModuleStateManagement moduleStateManagement;
        private readonly ModuleStore moduleStore;
        private readonly IServiceProvider serviceProvider;

        public LoraParameterViewModel(ModuleStateManagement moduleStateManagement, ModuleStore moduleStore, IServiceProvider serviceProvider)
        {
            this.moduleStateManagement = moduleStateManagement;
            this.moduleStore = moduleStore;
            this.serviceProvider = serviceProvider;

            this.moduleStateManagement.LoraParamsCreated += OnCreateLoraParameter;
            this.moduleStateManagement.OpenUpdateLoraParams += OnOpenUpdateLoraParamter; // load on program
            this.moduleStateManagement.UpdateParamsOfModule += OnUpdateParamsOfModule;
            this.moduleStateManagement.ReadLoraConfigParams += OnReadConfigLoraParameter;// load from database
            /*            this.moduleStateManagement.ConfigParams += OnConfigParameterToHardware;*/

            ListPower = new ObservableCollection<string>() { "20", "17", "14", "10" };
            ListAirRate = new ObservableCollection<string>() { "0.3", "1.2", "2.4", "4.8", "9.6", "19.2" };
            ListUartRate = new ObservableCollection<string>() { "1200", "2400", "4800", "9600", "19200", "38400", "57600", "115200" };
            ListWORTime = new ObservableCollection<string>() { "250", "500", "750", "1000", "1250", "1500", "1750", "2000" };
            ListParity = new ObservableCollection<string>() { "8N1", "8O1", "8E1" };
            ListFixedMode = ListIOMode = ListFEC = new ObservableCollection<string>() { "0", "1" };

            //Event Delegate

            this.moduleStateManagement.ResetParameterModule += OnResetParameterModule;

        }

        private LoraParameterObject createLoraParamsObject()
        {
            try
            {
                Address = Address == null ? "FFFF" : Address;
                Channel = Channel == null ? "17" : Channel;
                UartRate = UartRate == null ? "9600" : UartRate;
                AirRate = AirRate == null ? "2.4" : AirRate;
                PowerTransmit = PowerTransmit == null ? "20" : PowerTransmit;
                FixedMode = FixedMode == null ? "0" : FixedMode;
                WORTime = WORTime == null ? "250" : WORTime;
                Parity = Parity == null ? "8N1" : Parity;
                IOMode = IOMode == null ? "1" : IOMode;
                FEC = FEC == null ? "2.4" : FEC;

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
                    FEC = FEC,
                    DestinationAddress = DestinationAddress,
                    DestinationChannel = DestinationChannel
                };
            }
            catch (Exception e)
            {

                MessageBox.Show("Lora paramter view model " + "createLoraParamsObject " + e);
                return null;
            }

        }
        private void OnCreateLoraParameter(ModuleObject module)
        {
            try
            {
                var loraParams = createLoraParamsObject();
                module.parameters = loraParams;
                module.type = "lora";

                /*            moduleStore.ModuleObjects.Add(module);*/

                var result = serviceProvider.GetRequiredService<IEnvironmentService>().configHardware(module.port, new
                {
                    module = module.type,
                    id = module.id,
                    baudrate = loraParams.UartRate
                });
                if (result)
                {
                    moduleStateManagement.configHardwareSuccess(module);
                    MessageBox.Show("config object success!");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Lora paramter view model " + "OnCreateLoraParameter " + e);
            }
        }
        private void OnUpdateParamsOfModule(ModuleObject moduleObject)
        {
            try
            {
                if (moduleObject.type == "lora")
                {
                    var loraParams = createLoraParamsObject();
                    moduleObject.parameters = loraParams;

                    var result = serviceProvider.GetRequiredService<IEnvironmentService>().configHardware(moduleObject.port, new
                    {
                        module = moduleObject.type,
                        id = moduleObject.id,
                        baudrate = loraParams.UartRate
                    });
                    if (result)
                    {
                        moduleStateManagement.configHardwareSuccess(moduleObject);
                        MessageBox.Show("config object success!");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Lora paramter view model " + "OnUpdateParamsOfModule " + e);
            }
            
        }
        private void OnOpenUpdateLoraParamter(LoraParameterObject loraParams)
        {
            try
            {
                Address = loraParams.Address;
                Channel = loraParams.Channel;
                AirRate = loraParams.AirRate;
                PowerTransmit = loraParams.Power;
                FixedMode = loraParams.FixedMode;
                WORTime = loraParams.WORTime;
                Parity = loraParams.Parity;
                IOMode = loraParams.IOMode;
                FEC = loraParams.FEC;
                UartRate = loraParams.UartRate;
                DestinationChannel = loraParams.DestinationChannel;
                DestinationAddress = loraParams.DestinationAddress;
            }
            catch (Exception e)
            {
                MessageBox.Show("Lora paramter view model " + "OnOpenUpdateLoraParamter " + e);
            }
            
        }
        private void OnReadConfigLoraParameter(Dictionary<string, string> listParams)
        {
            try
            {
                Address = listParams["Address"];
                Channel = listParams["Channel"];
                AirRate = listParams["AirRate"];
                PowerTransmit = listParams["Power"];
                FixedMode = listParams["FixedMode"];
                WORTime = listParams["WORTime"];
                Parity = listParams["Parity"];
                IOMode = listParams["IOMode"];
                FEC = listParams["FEC"];
                UartRate = listParams["UartRate"];
                DestinationAddress = listParams["DestinationAddress"];
                DestinationChannel = listParams["DestinationChannel"];
            }
            catch (Exception e)
            {
                MessageBox.Show("Lora paramter view model " + "OnReadConfigLoraParameter " + e);
            }
            
        }
        private void OnResetParameterModule()
        {
            Address = null;
            Channel = null;
            AirRate = null;
            PowerTransmit = null;
            FixedMode = null;
            WORTime = null;
            Parity = null;
            IOMode = null;
            FEC = null;
            UartRate = null;
            DestinationChannel = null;
            DestinationAddress = null;
        }
    }
}
