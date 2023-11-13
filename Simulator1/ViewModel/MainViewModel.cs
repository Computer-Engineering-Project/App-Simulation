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
using Simulator1.Model;
using Simulator1.Store;
using Environment.Service.Interface;

namespace Simulator1.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private ObservableCollection<ModuleObject> moduleObjects;
        public ObservableCollection<ModuleObject> ModuleObjects { get => moduleObjects; set { moduleObjects = value; OnPropertyChanged(); } }

        private ObservableCollection<string> ports;
        public ObservableCollection<string> Ports { get => ports;set { ports = value;OnPropertyChanged(); } }

        private bool isDialogOpen = false;
        public bool IsDialogOpen { get => isDialogOpen; set { isDialogOpen = value; OnPropertyChanged(); } }

        private string testText;
        public string TestText { get => testText; set { testText = value; OnPropertyChanged(); } }

        private List<string> testports = new List<string>() { "COM5", "COM6" };


        private readonly MainStore mainStore;
        private readonly ModuleStore moduleStore;
        private readonly ModuleStateManagement moduleStateManagement;
        private readonly IEnvironmentService enviromentService;

        public BaseViewModel CurrentModuleViewModel => mainStore.CurrentViewModel;
        /*public ModuleParameterViewModel ModuleParameterViewModel { get => moduleParameterViewModel; set { moduleParameterViewModel = value; OnPropertyChanged(); } }*/
        /*private ModuleParameterStore moduleParameterStore = new ModuleParameterStore();*/
        /*public ModuleParameterStore ModuleParameterStore { get => moduleParameterStore; set { moduleParameterStore = value; OnPropertyChanged(); } }*/
        public ICommand OpenDialogCommand { get; set; }
        public ICommand SavePositionCommand { get; set; }

        ~MainViewModel() { }
        public MainViewModel(MainStore mainStore, ModuleStateManagement moduleStateManagement, ModuleStore moduleStore, IEnvironmentService environmentService)
        {
            this.mainStore = mainStore;
            this.moduleStore = moduleStore;
            this.moduleStateManagement = moduleStateManagement;
            this.enviromentService = environmentService;
            moduleObjects = new ObservableCollection<ModuleObject>();
            this.moduleStateManagement.ModuleObjectCreated += OnModuleObjectCreated;
            ports = new ObservableCollection<string>(/**/ testports);

            OpenDialogCommand = new ParameterRelayCommand<string>((p) => { return true; }, (port) => OpenDialog(port));
            SavePositionCommand = new ParameterRelayCommand<string>((port) => { return true; }, (port) =>
            {
                ExecuteTest(port);
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
                ((ModuleParameterViewModel)CurrentModuleViewModel).Port = port;
                ((ModuleParameterViewModel)CurrentModuleViewModel).Save += CloseDialog;
            }
        }
        private void ExecuteTest(string port)
        {
            TestText= port;
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
                if(CurrentModuleViewModel is ModuleParameterViewModel)
                {
                    ((LoraParameterViewModel)((ModuleParameterViewModel)CurrentModuleViewModel).CurrentModuleViewModel).UartRate = matchParams.UartRate;
                    ((LoraParameterViewModel)((ModuleParameterViewModel)CurrentModuleViewModel).CurrentModuleViewModel).PowerTransmit = matchParams.Power;
                    ((LoraParameterViewModel)((ModuleParameterViewModel)CurrentModuleViewModel).CurrentModuleViewModel).IOMode = matchParams.IOMode;
                    ((LoraParameterViewModel)((ModuleParameterViewModel)CurrentModuleViewModel).CurrentModuleViewModel).FixedMode = matchParams.FixedMode;
                    ((LoraParameterViewModel)((ModuleParameterViewModel)CurrentModuleViewModel).CurrentModuleViewModel).WORTime = matchParams.WORTime;
                    ((LoraParameterViewModel)((ModuleParameterViewModel)CurrentModuleViewModel).CurrentModuleViewModel).Address = matchParams.Address;
                    ((LoraParameterViewModel)((ModuleParameterViewModel)CurrentModuleViewModel).CurrentModuleViewModel).AirRate = matchParams.AirRate;
                    ((LoraParameterViewModel)((ModuleParameterViewModel)CurrentModuleViewModel).CurrentModuleViewModel).Channel = matchParams.Channel;
                    ((LoraParameterViewModel)((ModuleParameterViewModel)CurrentModuleViewModel).CurrentModuleViewModel).FEC = matchParams.FEC;
                    ((LoraParameterViewModel)((ModuleParameterViewModel)CurrentModuleViewModel).CurrentModuleViewModel).Parity = matchParams.Parity;
                }
            }
        }
        private void CloseDialog()
        {
            IsDialogOpen = false;
        }
        public override void Dispose()
        {
            moduleStateManagement.ModuleObjectCreated -= OnModuleObjectCreated;
            base.Dispose();
        }

    }
}
