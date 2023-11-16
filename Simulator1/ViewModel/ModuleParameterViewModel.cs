using Environment.Model.Module;
using Environment.Service.Interface;
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

        public BaseViewModel CurrentModuleViewModel => moduleParamViewStore.CurrentViewModel;

        #endregion

        #region command
        public ICommand generateModuleCommand { get; set; }
        public ICommand CloseDialogCommand { get; set; }
        public ICommand LoraParamCommand { get; set; }
        public ICommand ZigbeeParamCommand { get; set; }

        public ICommand ActiveCommand { get; set; }
        public ICommand ReadConfigCommand { get; set; }
        public ICommand ConfigCommand { get; set; }

        #endregion

        private readonly ModuleParameterViewStore moduleParamViewStore;
        private readonly ModuleStore moduleStore;
        private readonly ModuleStateManagement moduleStateManagement;
        private readonly INavigateService loraParameterNavigateService;
        private readonly INavigateService zigbeeParameterNavigateService;
        private readonly IEnvironmentService environmentService;

        ~ModuleParameterViewModel() { }
        public ModuleParameterViewModel(ModuleParameterViewStore moduleParamViewStore, ModuleStateManagement moduleStateManagement, ModuleStore moduleStore,
            IEnvironmentService environmentService,
            INavigateService loraParameterNavigateService, INavigateService zigbeeParameterNavigateService)
        {
            //DI
            this.moduleParamViewStore = moduleParamViewStore;
            this.moduleStore = moduleStore;
            this.moduleStateManagement = moduleStateManagement;
            this.loraParameterNavigateService = loraParameterNavigateService;
            this.zigbeeParameterNavigateService = zigbeeParameterNavigateService;
            this.environmentService = environmentService;
            //Navigate
            LoraParamCommand = new NavigateCommand(this.loraParameterNavigateService);
            ZigbeeParamCommand = new NavigateCommand(this.zigbeeParameterNavigateService);
            //UI/UX
            generateModuleCommand = new RelayCommand(() => GenerateModule());
            CloseDialogCommand = new RelayCommand(() => Save?.Invoke(Port));
            //Environment
            ActiveCommand = new RelayCommand(() => ExecuteActiveHardware());
            ReadConfigCommand = new RelayCommand(() => ExecuteReadConfigFromHardware());

            //Event delegate
            this.moduleParamViewStore.CurrentModuleViewModelChanged += OnCurrentViewModelChanged;
        }
        private void GenerateModule()
        {
            if (!string.IsNullOrEmpty(HorizontalX) && !string.IsNullOrEmpty(VerticalY))
            {
                var x = Convert.ToInt32(HorizontalX);
                var y = Convert.ToInt32(VerticalY);
                var numOfModule = moduleStore.ModuleObjects.Count;
                var module = new ModuleObject()
                {
                    port = Port,
                    x = x,
                    y = y,
                };
                moduleStateManagement.createLoraParameter(module);
                moduleStateManagement.createModuleObject();
            }

        }

        private void ExecuteActiveHardware()
        {
            environmentService.ActiveHardware(Port);
        }
        private void ExecuteReadConfigFromHardware()
        {
            var parameters = moduleStore.LoadParametersFromHardware(Port);
            string json = JsonConvert.SerializeObject(parameters);
            Dictionary<string, string> listParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            moduleStateManagement.readLoraConfigParameter(listParams);
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentModuleViewModel));
        }
        public override void Dispose()
        {
            base.Dispose();
        }
        
    }
}
