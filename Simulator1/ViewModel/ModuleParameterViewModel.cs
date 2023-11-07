using Environment.Model.Module;
using Environment.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xaml.Behaviors;
using Newtonsoft.Json;
using Simulator1.Service;
using Simulator1.State_Management;
using Simulator1.Store;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace Simulator1.ViewModel
{
    public class ModuleParameterViewModel : BaseViewModel
    {
        public event Action<string> Save;

        #region variable

        private string horizontal_x;
        public string HorizontalX { get => horizontal_x; set { horizontal_x = value; OnPropertyChanged(); } }

        private string vertical_y;
        public string VerticalY { get => vertical_y; set { vertical_y = value; OnPropertyChanged(); } }

        private string port;
        public string Port { get => port; set { port = value; OnPropertyChanged(); } }

        private string isUpdate;
        public string IsUpdate { get => isUpdate; set { isUpdate = value; OnPropertyChanged(); } }

        private string moduleType;
        public string ModuleType { get => moduleType; set { moduleType = value; OnPropertyChanged(); } }

        private bool isEnableSave;
        public bool IsEnableSave { get => isEnableSave; set { isEnableSave = value; OnPropertyChanged(); } }

        public BaseViewModel CurrentModuleViewModel => moduleParamViewStore.CurrentViewModel;

        #endregion

        #region command
        public ICommand generateModuleCommand { get; set; }
        public ICommand CloseDialogCommand { get; set; }
        public ICommand LoraParamCommand { get; set; }
        public ICommand ZigbeeParamCommand { get; set; }
        public ICommand LoraModuleCommand { get; set; }
        public ICommand ZigbeeModuleCommand { get; set; }

        public ICommand ActiveCommand { get; set; }
        public ICommand ReadConfigCommand { get; set; }
        public ICommand ConfigCommand { get; set; }

        #endregion

        private readonly ModuleParameterViewStore moduleParamViewStore;
        private readonly ModuleStore moduleStore;
        private readonly ModuleStateManagement moduleStateManagement;
        private readonly INavigateService loraParameterNavigateService;
        private readonly INavigateService zigbeeParameterNavigateService;
        private readonly IServiceProvider serviceProvider;

        ~ModuleParameterViewModel() { }
        public ModuleParameterViewModel(ModuleParameterViewStore moduleParamViewStore, ModuleStateManagement moduleStateManagement, ModuleStore moduleStore,
            IServiceProvider serviceProvider,
            INavigateService loraParameterNavigateService, INavigateService zigbeeParameterNavigateService)
        {
            IsEnableSave = false;
            //DI
            this.moduleParamViewStore = moduleParamViewStore;
            this.moduleStore = moduleStore;
            this.moduleStateManagement = moduleStateManagement;
            this.loraParameterNavigateService = loraParameterNavigateService;
            this.zigbeeParameterNavigateService = zigbeeParameterNavigateService;
            this.serviceProvider = serviceProvider;
            //Navigate
            LoraParamCommand = new NavigateCommand(this.loraParameterNavigateService);
            ZigbeeParamCommand = new NavigateCommand(this.zigbeeParameterNavigateService);
            LoraModuleCommand = new RelayCommand(() => ExecuteChangeModuleType("lora"));
            ZigbeeModuleCommand = new RelayCommand(() => ExecuteChangeModuleType("zigbee"));
            //UI/UX
            generateModuleCommand = new RelayCommand(() => GenerateModule());
            CloseDialogCommand = new RelayCommand(() => CloseModule());
            //Environment
            ActiveCommand = new RelayCommand(() => ExecuteActiveHardware());
            ReadConfigCommand = new RelayCommand(() => ExecuteReadConfigFromHardware());
            ConfigCommand = new RelayCommand(() => ExecuteConfigHardware());

            //Event delegate
            this.moduleParamViewStore.CurrentModuleViewModelChanged += OnCurrentViewModelChanged;
            this.moduleStateManagement.UpdatePositionAndPort += OnUpdatePositionAndPort;
            this.moduleStateManagement.IsActionUpdate += OnIsActionUpdate;
            this.moduleStateManagement.ConfigHardwareSuccess += OnConfigHardwareSuccess;
        }
        private void ExecuteChangeModuleType(string type)
        {
            ModuleType = type;
        }
        private void GenerateModule()
        {
            if (IsUpdate != "true" && !string.IsNullOrEmpty(HorizontalX) && !string.IsNullOrEmpty(VerticalY))
            {
                var x = Double.Parse(HorizontalX);
                var y = Double.Parse(VerticalY);
                foreach (var module in moduleStore.ModuleObjects)
                {
                    if (module.port == Port)
                    {
                        module.x = x;
                        module.y = y;
                    }
                }
                moduleStateManagement.createModuleObject(Port);
            }
            else
            {
                var x = Double.Parse(HorizontalX);
                var y = Double.Parse(VerticalY);
                var module = new ModuleObject()
                {
                    port = Port,
                    x = x,
                    y = y,
                };
                moduleStateManagement.changePositionAndPort(module);
            }
            CloseModule();
        }

        private void CloseModule()
        {
            Save?.Invoke(Port);
            this.HorizontalX = null;
            this.VerticalY = null;
            this.moduleStateManagement.resetParameterModule();
        }
        private void ExecuteActiveHardware()
        {
            serviceProvider.GetRequiredService<IEnvironmentService>().ActiveHardware(Port);
        }
        private void ExecuteReadConfigFromHardware()
        {
            var parameters = moduleStore.LoadParametersFromHardware(Port);
            string json = JsonConvert.SerializeObject(parameters);
            Dictionary<string, string> listParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            moduleStateManagement.readLoraConfigParameter(listParams);
        }
        private void ExecuteConfigHardware()
        {
            var moduleObject = moduleStore.ModuleObjects.FirstOrDefault(x => x.port == Port);
            if (moduleObject != null)
            {
                moduleStateManagement.updateParamsOfModule(moduleObject);
            }
            else
            {
                var id = moduleStore.ModuleObjects.Count + 1;
                var module = new ModuleObject()
                {
                    port = Port,
                    id = id.ToString(),
                    mode = "MODE 3"
                };
                moduleStateManagement.createLoraParameter(module);
                /*                moduleStateManagement.configParameter(ModuleType);
                */
            }
        }
        private void OnConfigHardwareSuccess()
        {
            IsEnableSave = true;
        }
        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentModuleViewModel));
        }

        private void OnUpdatePositionAndPort(object moduleObject)
        {
            string json = JsonConvert.SerializeObject(moduleObject);
            Dictionary<string, string> listParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            HorizontalX = listParams["x"];
            VerticalY = listParams["y"];
            Port = listParams["port"];
        }
        private void OnIsActionUpdate(object isUpdate)
        {
            string json = JsonConvert.SerializeObject(isUpdate);
            Dictionary<string, string> listParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            IsUpdate = listParams["value"];
        }

        public override void Dispose()
        {
            base.Dispose();
        }

    }
}
