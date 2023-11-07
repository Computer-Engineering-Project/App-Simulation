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
using Environment.Model.History;

namespace Simulator1.ViewModel
{
    public class MainViewModel : BaseViewModel, ICommunication
    {
        private ObservableCollection<ModuleObject> moduleObjects;
        public ObservableCollection<ModuleObject> ModuleObjects { get => moduleObjects; set { moduleObjects = value; OnPropertyChanged(); } }

        private ObservableCollection<string> ports;
        public ObservableCollection<string> Ports { get => ports; set { ports = value; OnPropertyChanged(); } }

        private bool isDialogOpen = false;
        public bool IsDialogOpen { get => isDialogOpen; set { isDialogOpen = value; OnPropertyChanged(); } }

        private HistoryObject selectedItemHistory;
        public HistoryObject SelectedItemHistory { get => selectedItemHistory; set { selectedItemHistory = value; OnPropertyChanged(); } }

        /*private string testText;
        public string TestText { get => testText; set { testText = value; OnPropertyChanged(); } }*/

        private List<string> testports = new List<string>() {"COM6" };

        private ObservableCollection<HistoryObject> historyObjects;
        public ObservableCollection<HistoryObject> HistoryObjects { get => historyObjects; set { historyObjects = value; OnPropertyChanged(); } }

        private readonly MainStore mainStore;
        private readonly ModuleStore moduleStore;
        private readonly ModuleStateManagement moduleStateManagement;
        private readonly IServiceProvider serviceProvider;
        private readonly MainStateManagement mainStateManagement;
        private readonly testModuleViewModel testModuleVM;

        public BaseViewModel CurrentModuleViewModel => mainStore.CurrentViewModel;
        public BaseViewModel CurrentModuleObjectViewModel = new testModuleViewModel();
        /*public ModuleParameterViewModel ModuleParameterViewModel { get => moduleParameterViewModel; set { moduleParameterViewModel = value; OnPropertyChanged(); } }*/
        /*private ModuleParameterStore moduleParameterStore = new ModuleParameterStore();*/
        /*public ModuleParameterStore ModuleParameterStore { get => moduleParameterStore; set { moduleParameterStore = value; OnPropertyChanged(); } }*/
        public ICommand OpenDialogCommand { get; set; }
        public ICommand UpdateModuleCommand { get; set; }
        public ICommand LoadHistoryCommand { get; set; }
        public ICommand RunEnvironmentCommand { get; set; }

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
            ports = new ObservableCollection<string>(/**/ testports);
            HistoryObjects = new ObservableCollection<HistoryObject>()
            {
                new HistoryObject()
                {
                    Id = 1,
                    Source = "Hoang Dep Traiii",
                    Data = "0xxxxxxxxxxxxxxxxx",
                    DelayTime = "0xccccc"
                },
                new HistoryObject()
                {
                    Id = 2,
                    Source = "Hoang Dep Traiii",
                    Data = "0xxxxxxxxxxxxxxxxx",
                    DelayTime = "0xccccc"
                },
                new HistoryObject()
                {
                    Id = 3,
                    Source = "Hoang Dep Traiii",
                    Data = "0xxxxxxxxxxxxxxxxx",
                    DelayTime = "0xccccc"
                },
                new HistoryObject()
                {
                    Id = 1,
                    Source = "Hoang Dep Traiii",
                    Data = "0xxxxxxxxxxxxxxxxx",
                    DelayTime = "0xccccc"
                },
                new HistoryObject()
                {
                    Id = 1,
                    Source = "Hoang Dep Traiii",
                    Data = "0xxxxxxxxxxxxxxxxx",
                    DelayTime = "0xccccc"
                },
                new HistoryObject()
                {
                    Id = 1,
                    Source = "Hoang Dep Traiii",
                    Data = "0xxxxxxxxxxxxxxxxx",
                    DelayTime = "0xccccc"
                },
                new HistoryObject()
                {
                    Id = 1,
                    Source = "Hoang Dep Traiii",
                    Data = "0xxxxxxxxxxxxxxxxx",
                    DelayTime = "0xccccc"
                },
                new HistoryObject()
                {
                    Id = 1,
                    Source = "Hoang Dep Traiii",
                    Data = "0xxxxxxxxxxxxxxxxx",
                    DelayTime = "0xccccc"
                },
                new HistoryObject()
                {
                    Id = 1,
                    Source = "Hoang Dep Traiii",
                    Data = "0xxxxxxxxxxxxxxxxx",
                    DelayTime = "0xccccc"
                },
                new HistoryObject()
                {
                    Id = 1,
                    Source = "Hoang Dep Traiii",
                    Data = "0xxxxxxxxxxxxxxxxx",
                    DelayTime = "0xccccc"
                },new HistoryObject()
                {
                    Id = 1,
                    Source = "Hoang Dep Traiii",
                    Data = "0xxxxxxxxxxxxxxxxx",
                    DelayTime = "0xccccc"
                },
                new HistoryObject()
                {
                    Id = 1,
                    Source = "Hoang Dep Traiii",
                    Data = "0xxxxxxxxxxxxxxxxx",
                    DelayTime = "0xccccc"
                },
                new HistoryObject()
                {
                    Id = 1,
                    Source = "Hoang Dep Traiii",
                    Data = "0xxxxxxxxxxxxxxxxx",
                    DelayTime = "0xccccc"
                },
                new HistoryObject()
                {
                    Id = 1,
                    Source = "Hoang Dep Traiii",
                    Data = "0xxxxxxxxxxxxxxxxx",
                    DelayTime = "0xccccc"
                },
                new HistoryObject()
                {
                    Id = 1,
                    Source = "Hoang Dep Traiii",
                    Data = "0xxxxxxxxxxxxxxxxx",
                    DelayTime = "0xccccc"
                },
                new HistoryObject()
                {
                    Id = 1,
                    Source = "Hoang Dep Traiii",
                    Data = "0xxxxxxxxxxxxxxxxx",
                    DelayTime = "0xccccc"
                },
                new HistoryObject()
                {
                    Id = 1,
                    Source = "Hoang Dep Traiii",
                    Data = "0xxxxxxxxxxxxxxxxx",
                    DelayTime = "0xccccc"
                },
            };
            //Event delegate
            this.moduleStateManagement.ModuleObjectCreated += OnModuleObjectCreated;
            this.moduleStateManagement.ChangePositionAndPort += ExecuteAutoSavePosition;
            //Command
            OpenDialogCommand = new ParameterRelayCommand<string>((p) => { return true; }, (port) => OpenDialog(port));
            UpdateModuleCommand = new ParameterRelayCommand<string>((port) => { return true; }, (port) =>
            {   
                moduleStateManagement.isActionUpdate(new { 
                    value = true
                });
                ExecuteUpdateModule(port);
            });
            LoadHistoryCommand = new RelayCommand(() => ExecuteLoadHistory());
            autoSaveCommand = new ParameterRelayCommand<object>((o) => { return true; }, (o) =>
            {
                ExecuteAutoSavePosition(o);
            });

            SelectionChangedCommand = new RelayCommand(() => DectectActionSelectedColumn());

            RunEnvironmentCommand = new RelayCommand(() => ExecuteRunEnvironment());
        }
        //Delegate handler
        private void OnModuleObjectCreated()
        {
            ModuleObjects = new ObservableCollection<ModuleObject>(moduleStore.ModuleObjects);
        }
        //Command handler
        private void OpenDialog(string port)
        {
            IsDialogOpen = true;
            /*ModuleParameterViewModel = new ModuleParameterViewModel(moduleParamStore, moduleStateManagement, moduleStorePosition);
            ModuleParameterViewModel.Port = port;
            ModuleParameterViewModel.Save += CloseDialog;*/
            if (CurrentModuleViewModel is ModuleParameterViewModel)
            {
                serviceProvider.GetRequiredService<IEnvironmentService>().startPort(port);
                ((ModuleParameterViewModel)CurrentModuleViewModel).Port = port;
                ((ModuleParameterViewModel)CurrentModuleViewModel).Save += CloseDialog;
            }
        }
        private void ExecuteUpdateModule(string port)
        {
            /*TestText= port;*/
            IsDialogOpen = true;
            var modules = moduleStore.ModuleObjects;
            var matchParams = new LoraParameterObject();
            var moduleObject = new ModuleObject();
            foreach (var module in modules)
            {
                if (module.port == port)
                {
                    moduleObject = module;
                    matchParams = (LoraParameterObject)module.parameters;
                }
            }
            if (matchParams != null)
            {
                if (CurrentModuleViewModel is ModuleParameterViewModel)
                {
                    moduleStateManagement.updateLoraParameter(matchParams);
                    moduleStateManagement.updatePositionAndPort(new
                    {
                        port = port,
                        x = moduleObject.x,
                        y = moduleObject.y,

                    });
                }
            }
        }
        private void ExecuteLoadHistory()
        {
            mainStateManagement.loadHistoryFromDB();
            ModuleObjects = new ObservableCollection<ModuleObject>(moduleStore.ModuleObjects);
        }
        private void ExecuteAutoSavePosition(object positionObject)
        {
            string json = JsonConvert.SerializeObject(positionObject);
            Dictionary<string, string> listParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            foreach (var module in moduleStore.ModuleObjects)
            {
                if (module.port == listParams["port"])
                {
                    module.x = Double.Parse(listParams["x"]);
                    module.y = Double.Parse(listParams["y"]);
                }
            }

            ModuleObjects = new ObservableCollection<ModuleObject>(moduleStore.ModuleObjects);
        }
        private void CloseDialog(string port)
        {
            IsDialogOpen = false;
            serviceProvider.GetRequiredService<IEnvironmentService>().closePort(port);
        }
        private void ExecuteRunEnvironment()
        {
            serviceProvider.GetRequiredService<IEnvironmentService>().Run();
        }
        private void ExecuteChangeMode(string port, string mode)
        {
            serviceProvider.GetRequiredService<IEnvironmentService>().changeModeDevice(port, mode);
        }

        private void DectectActionSelectedColumn()
        {
            //todo
        }

        //Dispose
        public override void Dispose()
        {
            moduleStateManagement.ModuleObjectCreated -= OnModuleObjectCreated;
            base.Dispose();
        }

        //Received Data from Environment
        public void showQueueReceivedFromHardware(PacketTransferToView listTransferedPacket)
        {
            throw new NotImplementedException();
        }
    }
}
