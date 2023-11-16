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

namespace Simulator1.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private ObservableCollection<ModuleObject> moduleObjects;
        public ObservableCollection<ModuleObject> ModuleObjects { get => moduleObjects; set { moduleObjects = value; OnPropertyChanged(); } }

        private ObservableCollection<string> ports;
        public ObservableCollection<string> Ports { get => ports; set { ports = value; OnPropertyChanged(); } }

        private bool isDialogOpen = false;
        public bool IsDialogOpen { get => isDialogOpen; set { isDialogOpen = value; OnPropertyChanged(); } }

        /*private string testText;
        public string TestText { get => testText; set { testText = value; OnPropertyChanged(); } }*/

        private List<string> testports = new List<string>() { "COM5", "COM6" };


        private readonly MainStore mainStore;
        private readonly ModuleStore moduleStore;
        private readonly ModuleStateManagement moduleStateManagement;
        private readonly IEnvironmentService enviromentService;
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

        public ICommand testCommand { get; set; }
        public ICommand autoSaveCommand { get; set; }

        ~MainViewModel() { }
        public MainViewModel(MainStore mainStore, MainStateManagement mainStateManagement, ModuleStateManagement moduleStateManagement, ModuleStore moduleStore, IEnvironmentService environmentService, testModuleViewModel testModuleVM)
        {
            //DI
            this.mainStore = mainStore;
            this.moduleStore = moduleStore;
            this.moduleStateManagement = moduleStateManagement;
            this.enviromentService = environmentService;
            this.mainStateManagement = mainStateManagement;
            this.testModuleVM = testModuleVM;
            //Variable
            moduleObjects = new ObservableCollection<ModuleObject>();
            ports = new ObservableCollection<string>(/**/ testports);
            //Event delegate
            this.moduleStateManagement.ModuleObjectCreated += OnModuleObjectCreated;
            //Command
            OpenDialogCommand = new ParameterRelayCommand<string>((p) => { return true; }, (port) => OpenDialog(port));
            UpdateModuleCommand = new ParameterRelayCommand<string>((port) => { return true; }, (port) =>
            {
                ExecuteUpdateModule(port);
            });
            LoadHistoryCommand = new RelayCommand(() => ExecuteLoadHistory());
            autoSaveCommand = new ParameterRelayCommand<object>((o) => { return true; }, (o) =>
            {
                ExecuteAutoSavePosition(o);
            });
        }

        private void OnModuleObjectCreated()
        {
            ModuleObjects = new ObservableCollection<ModuleObject>(moduleStore.ModuleObjects);
        }
        private void OpenDialog(string port)
        {
            IsDialogOpen = true;
            /*ModuleParameterViewModel = new ModuleParameterViewModel(moduleParamStore, moduleStateManagement, moduleStorePosition);
            ModuleParameterViewModel.Port = port;
            ModuleParameterViewModel.Save += CloseDialog;*/
            if (CurrentModuleViewModel is ModuleParameterViewModel)
            {
                enviromentService.startPort(port);
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
            foreach (var module in modules)
            {
                if (module.port == port)
                {
                    matchParams = (LoraParameterObject)module.parameters;
                }
            }
            if (matchParams != null)
            {
                if (CurrentModuleViewModel is ModuleParameterViewModel)
                {
                    moduleStateManagement.updateLoraParameter(matchParams);
                    
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
        }
        private void CloseDialog(string port)
        {
            IsDialogOpen = false;
            enviromentService.closePort(port);
        }
        public override void Dispose()
        {
            moduleStateManagement.ModuleObjectCreated -= OnModuleObjectCreated;
            base.Dispose();
        }

    }
}
