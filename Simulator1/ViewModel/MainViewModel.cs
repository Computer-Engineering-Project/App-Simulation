using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Reflection;
using System.Collections.ObjectModel;
using Microsoft.Xaml.Behaviors;
using Simulator1.State_Management;
using Simulator1.Store;
using Environment.Service.Interface;
using Newtonsoft.Json;
using Environment.Model.Module;
using Microsoft.Extensions.DependencyInjection;
using Environment.Model.Packet;
using Environment.Model.ButtonPort;
using Environment.Model.History;

namespace Simulator1.ViewModel
{
    public class MainViewModel : BaseViewModel, ICommunication
    {
        private ObservableCollection<ModuleObject> moduleObjects;
        public ObservableCollection<ModuleObject> ModuleObjects { get => moduleObjects; set { moduleObjects = value; OnPropertyChanged(); } }

        private ObservableCollection<ButtonPort> ports;
        public ObservableCollection<ButtonPort> Ports { get => ports; set { ports = value; OnPropertyChanged(); } }

        private bool isDialogOpen = false;
        public bool IsDialogOpen { get => isDialogOpen; set { isDialogOpen = value; OnPropertyChanged(); } }

        private HistoryObject selectedItemHistory;
        public HistoryObject SelectedItemHistory { get => selectedItemHistory; set { selectedItemHistory = value; OnPropertyChanged(); } }
        //handle get value from object
        private string sourceHistory;
        public string SourceHistory { get => sourceHistory; set { sourceHistory = value; OnPropertyChanged(); } }
        private string dataHistory;
        public string DataHistory { get => dataHistory; set { dataHistory = value; OnPropertyChanged(); } }
        private string delayTimeHistory;
        public string DelayTimeHistory { get => delayTimeHistory; set { delayTimeHistory = value; OnPropertyChanged(); } }

        /*private string testText;
        public string TestText { get => testText; set { testText = value; OnPropertyChanged(); } }*/

        private List<string> testports = new List<string>() { "COM6" };

        private ObservableCollection<HistoryObject> historyObjects;
        public ObservableCollection<HistoryObject> HistoryObjects { get => historyObjects; set { historyObjects = value; OnPropertyChanged(); } }

        private readonly MainStore mainStore;
        private readonly ModuleStore moduleStore;
        private readonly ModuleStateManagement moduleStateManagement;
        private readonly IServiceProvider serviceProvider;
        private readonly MainStateManagement mainStateManagement;
        private readonly testModuleViewModel testModuleVM;

        public BaseViewModel CurrentModuleViewModel => mainStore.CurrentViewModel;
        /*public ModuleParameterViewModel ModuleParameterViewModel { get => moduleParameterViewModel; set { moduleParameterViewModel = value; OnPropertyChanged(); } }*/
        /*private ModuleParameterStore moduleParameterStore = new ModuleParameterStore();*/
        /*public ModuleParameterStore ModuleParameterStore { get => moduleParameterStore; set { moduleParameterStore = value; OnPropertyChanged(); } }*/
        public ICommand OpenDialogCommand { get; set; }
        public ICommand UpdateModuleCommand { get; set; }
        public ICommand LoadHistoryCommand { get; set; }
        public ICommand RunEnvironmentCommand { get; set; }
        public ICommand LoadPorts { get; set; }

        public ICommand testCommand { get; set; }
        public ICommand autoSaveCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }


        ~MainViewModel() { }
        public MainViewModel(MainStore mainStore, MainStateManagement mainStateManagement, ModuleStateManagement moduleStateManagement, ModuleStore moduleStore, IServiceProvider serviceProvider, testModuleViewModel testModuleVM)
        {

            //DI
            this.mainStore = mainStore;
            this.moduleStore = moduleStore;
            this.moduleStateManagement = moduleStateManagement;
            this.serviceProvider = serviceProvider;
            this.mainStateManagement = mainStateManagement;
            this.testModuleVM = testModuleVM;
            //Variable
            moduleObjects = new ObservableCollection<ModuleObject>();
            Ports = new ObservableCollection<ButtonPort>();
            HistoryObjects = new ObservableCollection<HistoryObject>()
            /*{
                new HistoryObject()
                {
                    Id = 1,
                    Source = "1234567890",
                    Data = "0xxxxxxxxxxx",
                    DelayTime = "0x101010"
                },
                new HistoryObject()
                {
                    Id = 2,
                    Source = "0987654321",
                    Data = "0xxxxxxxxx",
                    DelayTime = "0x56767"
                },
                new HistoryObject()
                {
                    Id = 3,
                    Source = "1232132",
                    Data = "0xxxxxxx",
                    DelayTime = "0xbgbgdb"
                },
            }*/;
            //Event delegate
            this.moduleStateManagement.ModuleObjectCreated += OnModuleObjectCreated;
            this.moduleStateManagement.ChangePositionAndPort += ExecuteAutoSavePosition;
            this.moduleStateManagement.DeleteModule += OnDeleteModule;
            //Command
            OpenDialogCommand = new ParameterRelayCommand<string>((p) => { return true; }, (port) => ExecuteClickPort(port));
            UpdateModuleCommand = new ParameterRelayCommand<ModuleObject>((module) => { return true; }, (module) =>
            {
                moduleStateManagement.isActionUpdate(new
                {
                    value = true
                });
                ExecuteOpenUpdateModule(module);
            });
            LoadHistoryCommand = new RelayCommand(() => ExecuteLoadHistory());
            autoSaveCommand = new ParameterRelayCommand<object>((o) => { return true; }, (o) =>
            {
                ExecuteAutoSavePosition(o);
            });

            SelectionChangedCommand = new RelayCommand(() => DectectActionSelectedColumn());

            RunEnvironmentCommand = new RelayCommand(() => ExecuteRunEnvironment());
            LoadPorts = new RelayCommand(() => ExecuteLoadPorts());
        }
        //Delegate handler
        private void OnModuleObjectCreated(string portName)
        {
            ModuleObjects = new ObservableCollection<ModuleObject>(moduleStore.ModuleObjects);
            var ports = Ports;
            foreach (var p in ports)
            {
                if (p.portName == portName)
                {
                    p.color = "LightGreen";
                }
            }
            Ports = new ObservableCollection<ButtonPort>(ports);
        }
        private void ExecuteAutoSavePosition(object positionObject)
        {
            string json = JsonConvert.SerializeObject(positionObject);
            Dictionary<string, string> listParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            foreach (var module in moduleStore.ModuleObjects)
            {
                if (listParams["id"] != null)
                {
                    if (module.id == listParams["id"])
                    {
                        var x = Double.Parse(listParams["x"]);
                        if (x < 0) x = 0;
                        module.x = x;
                        var y = Double.Parse(listParams["y"]);
                        if (y < 0) y = 0;
                        module.y = y;
                    }
                }
                else
                {
                    if (module.port == listParams["port"])
                    {
                        var x = Double.Parse(listParams["x"]);
                        if (x < 0) x = 0;
                        module.x = x;
                        var y = Double.Parse(listParams["y"]);
                        if (y < 0) y = 0;
                        module.y = y;
                    }
                }

            }

            ModuleObjects = new ObservableCollection<ModuleObject>(moduleStore.ModuleObjects);
        }
        private void OnDeleteModule(string portName)
        {
            var tmp_ports = Ports;
            var p = tmp_ports.FirstOrDefault(x => x.portName == portName);
            if (p != null)
            {
                p.color = "Wheat";
            }
            Ports = tmp_ports;
        }
        //Command handler
        private void ExecuteClickPort(string portName)
        {
            var portObject = Ports.FirstOrDefault(x => x.portName == portName);
            if (portObject != null)
            {
                if (portObject.color == "Wheat")
                {
                    OpenDialog(portName);
                }
                else
                {
                    //Binding history data to history table
                    
                }
            }
        }
        private void OpenDialog(string portName)
        {
            IsDialogOpen = true;
            /*ModuleParameterViewModel = new ModuleParameterViewModel(moduleParamStore, moduleStateManagement, moduleStorePosition);
            ModuleParameterViewModel.Port = port;
            ModuleParameterViewModel.Save += CloseDialog;*/
            var tmp_Ports = Ports.Select(x => x.portName).ToList();
            foreach (var port in Ports)
            {
                if (port.color == "LightGreen")
                {
                    tmp_Ports.Remove(port.portName);
                }
            }
            if (CurrentModuleViewModel is ModuleParameterViewModel)
            {
                //serviceProvider.GetRequiredService<IEnvironmentService>().startPort(portName);
                ((ModuleParameterViewModel)CurrentModuleViewModel).Port = portName;
                ((ModuleParameterViewModel)CurrentModuleViewModel).ListPort = new ObservableCollection<string>(tmp_Ports);
                ((ModuleParameterViewModel)CurrentModuleViewModel).Save += CloseDialog;
                ((ModuleParameterViewModel)CurrentModuleViewModel).IsEnableDelete = false;
                ((ModuleParameterViewModel)CurrentModuleViewModel).IsEnablePortSelect = false;
            }
        }
        private void ExecuteOpenUpdateModule(ModuleObject module)
        {
            /*TestText= port;*/
            IsDialogOpen = true;
            var tmp_Ports = Ports.Select(x => x.portName).ToList();
            foreach (var port in Ports)
            {
                if (port.color == "LightGreen")
                {
                    tmp_Ports.Remove(port.portName);
                }
            }
            if (CurrentModuleViewModel is ModuleParameterViewModel)
            {
                ((ModuleParameterViewModel)CurrentModuleViewModel).Id = module.id;
                ((ModuleParameterViewModel)CurrentModuleViewModel).IsEnableDelete = true;
                if (module.port != null)
                {
                    //serviceProvider.GetRequiredService<IEnvironmentService>().startPort(module.port);
                    ((ModuleParameterViewModel)CurrentModuleViewModel).Port = module.port;
                    ((ModuleParameterViewModel)CurrentModuleViewModel).IsEnablePortSelect = false;
                }
                else
                {
                    ((ModuleParameterViewModel)CurrentModuleViewModel).IsEnablePortSelect = true;
                }
                ((ModuleParameterViewModel)CurrentModuleViewModel).ListPort = new ObservableCollection<string>(tmp_Ports);
                ((ModuleParameterViewModel)CurrentModuleViewModel).Save += CloseDialog;
            }
            if (module.parameters != null)
            {
                string jsonParameters = JsonConvert.SerializeObject(module.parameters);
                if (module.type == "lora")
                {
                    var loraParameters = JsonConvert.DeserializeObject<LoraParameterObject>(jsonParameters);
                    if (CurrentModuleViewModel is ModuleParameterViewModel)
                    {
                        moduleStateManagement.openUpdateLoraParameter(loraParameters);
                        moduleStateManagement.updatePosition(new
                        {
                            x = module.x,
                            y = module.y,

                        });
                    }
                }
            }
        }
        private void ExecuteLoadHistory()
        {
            mainStateManagement.loadHistoryFromDB();
            ModuleObjects = new ObservableCollection<ModuleObject>(moduleStore.ModuleObjects);
        }

        private void CloseDialog(string port)
        {
            IsDialogOpen = false;
            ModuleObjects = new ObservableCollection<ModuleObject>(moduleStore.ModuleObjects);
            if(port!= null)
            {
                serviceProvider.GetRequiredService<IEnvironmentService>().closePort(port);
            }
        }
        private void ExecuteLoadPorts()
        {
            var ports = serviceProvider.GetRequiredService<IEnvironmentService>().loadPorts();
            var previousPorts = Ports.Select(x => x.portName).ToList();
            var tmpPorts = Ports;
            if (ports != null)
            {
                var addPorts = ports.Except(previousPorts);
                var removePorts = previousPorts.Except(ports);
                foreach (var port in addPorts)
                {
                    tmpPorts.Add(new ButtonPort()
                    {
                        color = "Wheat",
                        portName = port
                    });
                    moduleStore.Ports.Add(port);
                }
                foreach (var port in removePorts)
                {
                    var _object = tmpPorts.Where(x => x.portName == port).FirstOrDefault();
                    if (_object != null)
                    {
                        tmpPorts.Remove(_object);
                        moduleStore.Ports.Remove(port);
                    }
                }
                Ports = tmpPorts;

            }
        }

        private void DectectActionSelectedColumn()
        {
            //todo

            SourceHistory = SelectedItemHistory.Source;
            DataHistory = SelectedItemHistory.Data;
            DelayTimeHistory = SelectedItemHistory.DelayTime;
        }

        //Dispose
        public override void Dispose()
        {
            moduleStateManagement.ModuleObjectCreated -= OnModuleObjectCreated;
            base.Dispose();
        }
        private void ExecuteRunEnvironment()
        {
            serviceProvider.GetRequiredService<IEnvironmentService>().passModuleObjects(new List<ModuleObject>(ModuleObjects));
            serviceProvider.GetRequiredService<IEnvironmentService>().Run();
        }
        //Received Data from Environment
        public void showQueueReceivedFromHardware(PacketTransferToView listTransferedPacket)
        {
            throw new NotImplementedException();
        }

        public void deviceChangeMode(int mode, string port)
        {
            throw new NotImplementedException();
        }
    }
}
