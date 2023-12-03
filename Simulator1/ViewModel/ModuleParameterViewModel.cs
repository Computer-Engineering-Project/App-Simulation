using Environment.Model.History;
using Environment.Model.Module;
using Environment.Model.VMParameter;
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
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Channels;
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
        public string HorizontalX { get => horizontal_x; set { horizontal_x = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private string vertical_y;
        public string VerticalY { get => vertical_y; set { vertical_y = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private ObservableCollection<string> listPort;
        public ObservableCollection<string> ListPort { get => listPort; set { listPort = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private string port;
        public string Port { get => port; set { port = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private string id;
        public string Id { get => id; set { id = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private string isUpdate;
        public string IsUpdate { get => isUpdate; set { isUpdate = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private string moduleType;
        public string ModuleType { get => moduleType; set { moduleType = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private ModuleObject tmp_moduleObject;
        public ModuleObject tmp_ModuleObject { get => tmp_moduleObject; set { tmp_moduleObject = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private bool isEnableSave = false;
        public bool IsEnableSave { get => isEnableSave; set { isEnableSave = value; OnPropertyChanged(); } }

        private bool isEnableDelete = false;
        public bool IsEnableDelete { get => isEnableDelete; set { isEnableDelete = value; OnPropertyChanged(); } }

        private bool isEnablePortSelect = false;
        public bool IsEnablePortSelect { get => isEnablePortSelect; set { isEnablePortSelect = value; OnPropertyChanged(); } }

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
        public ICommand DeleteDialogCommand { get; set; }

        #endregion

        private readonly ModuleParameterViewStore moduleParamViewStore;
        private readonly ModuleStore moduleStore;
        private readonly ModuleStateManagement moduleStateManagement;
        private readonly INavigateService loraParameterNavigateService;
        private readonly INavigateService zigbeeParameterNavigateService;
        private readonly IServiceProvider serviceProvider;
        private readonly HistoryDataStore historyDataStore;
        private readonly HistoryStateManagement historyStateManagement;
        private readonly StatusStateManagement statusStateManagement;

        ~ModuleParameterViewModel() { }
        public ModuleParameterViewModel(ModuleParameterViewStore moduleParamViewStore, ModuleStateManagement moduleStateManagement, ModuleStore moduleStore,
            IServiceProvider serviceProvider, HistoryDataStore historyDataStore, HistoryStateManagement historyStateManagement, StatusStateManagement statusStateManagement,
            INavigateService loraParameterNavigateService, INavigateService zigbeeParameterNavigateService)
        {

            //DI
            this.moduleParamViewStore = moduleParamViewStore;
            this.moduleStore = moduleStore;
            this.moduleStateManagement = moduleStateManagement;
            this.loraParameterNavigateService = loraParameterNavigateService;
            this.zigbeeParameterNavigateService = zigbeeParameterNavigateService;
            this.serviceProvider = serviceProvider;
            this.historyDataStore = historyDataStore;
            this.historyStateManagement = historyStateManagement;
            this.statusStateManagement = statusStateManagement;
            //Navigate
            LoraParamCommand = new NavigateCommand(this.loraParameterNavigateService);
            ZigbeeParamCommand = new NavigateCommand(this.zigbeeParameterNavigateService);
            LoraModuleCommand = new RelayCommand(() => ExecuteChangeModuleType("lora"));
            ZigbeeModuleCommand = new RelayCommand(() => ExecuteChangeModuleType("zigbee"));
            //UI/UX
            generateModuleCommand = new RelayCommand(() => GenerateModule());
            CloseDialogCommand = new RelayCommand(() => CloseModule());
            DeleteDialogCommand = new RelayCommand(() => DeleteModule());
            //Environment
            ActiveCommand = new RelayCommand(() => ExecuteActiveHardware());
            ReadConfigCommand = new RelayCommand(() => ExecuteReadConfigFromHardware());
            ConfigCommand = new RelayCommand(() => ExecuteConfigHardware());

            //Event delegate
            this.moduleParamViewStore.CurrentModuleViewModelChanged += OnCurrentViewModelChanged;
            this.moduleStateManagement.UpdatePosition += OnUpdatePosition;
            this.moduleStateManagement.IsActionUpdate += OnIsActionUpdate;
            this.moduleStateManagement.ConfigHardwareSuccess += OnConfigHardwareSuccess;
            this.moduleStateManagement.UpdateParamsForModuleParameterVM += OnUpdateParamterForVM;
        }
        private void OnUpdateParamterForVM(ModuleParameterVM moduleParameterVM)
        {
            try
            {
                if (moduleParameterVM != null)
                {
                    HorizontalX = moduleParameterVM.horizontal_x;
                    VerticalY = moduleParameterVM.vertical_y;
                    ListPort = new ObservableCollection<string>(moduleParameterVM.listPort);
                    Port = moduleParameterVM.port;
                    Id = moduleParameterVM.id;
                    IsUpdate = moduleParameterVM.isUpdate;
                    ModuleType = moduleParameterVM.moduleType;
                    tmp_ModuleObject = moduleParameterVM.tmp_moduleObject;
                    IsEnableSave = moduleParameterVM.isEnableSave;
                    IsEnableDelete = moduleParameterVM.isEnableDelete;
                    IsEnablePortSelect = moduleParameterVM.isEnablePortSelect;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Module paramter view model " + "OnUpdateParamterForVM " + e);
            }
            
        }
        private void ExecuteChangeModuleType(string type)
        {
            ModuleType = type;
        }
        private void GenerateModule()
        {
            try
            {
                var random = new Random();
                if (string.IsNullOrEmpty(HorizontalX))
                {
                    HorizontalX = random.Next(501).ToString();
                }
                if (string.IsNullOrEmpty(VerticalY))
                {
                    VerticalY = random.Next(501).ToString();
                }
                if (IsUpdate != "true")
                {
                    var x = Double.Parse(HorizontalX);
                    if (x < 0) x = 0;
                    var y = Double.Parse(VerticalY);
                    if (y < 0) y = 0;
                    tmp_ModuleObject.x = x;
                    tmp_ModuleObject.y = y;
                    moduleStore.ModuleObjects.Add(tmp_ModuleObject);
                    historyDataStore.ModuleHistories.Add(new ModuleHistory()
                    {
                        moduleObject = tmp_ModuleObject,
                    });
                    moduleStateManagement.createModuleObject(Port);
                }
                else
                {
                    var x = Double.Parse(HorizontalX);
                    if (x < 0) x = 0;
                    var y = Double.Parse(VerticalY);
                    if (y < 0) y = 0;
                    var module = new ModuleObject()
                    {
                        port = Port,
                        x = x,
                        y = y,
                    };
                    foreach (var m in moduleStore.ModuleObjects)
                    {
                        if (m.id == tmp_ModuleObject.id)
                        {
                            if (tmp_ModuleObject.port == null)
                            {
                                tmp_ModuleObject.port = ListPort[0];
                                module.port = ListPort[0];
                            }
                            m.port = tmp_ModuleObject.port;
                            m.y = tmp_ModuleObject.y;
                            m.x = tmp_ModuleObject.x;
                            m.mode = tmp_ModuleObject.mode;
                            m.parameters = tmp_ModuleObject.parameters;
                            m.type = tmp_ModuleObject.type;
                            historyStateManagement.reloadHistoryData(m);
                        }
                    }
                    moduleStateManagement.changePositionAndPort(module);
                    moduleStateManagement.createModuleObject(module.port);
                }

                CloseModule();
            }
            catch (Exception e)
            {
                MessageBox.Show("Module paramter view model " + "GenerateModule " + e);
            }
            
        }
        private void DeleteModule()
        {
            try
            {
                var module = moduleStore.ModuleObjects.FirstOrDefault(x => x.id == Id);
                if (module != null)
                {
                    Reset();
                    this.moduleStateManagement.resetParameterModule();
                    moduleStateManagement.deleteModule(module.port);
                    moduleStore.ModuleObjects.Remove(module);
                    Save?.Invoke(module.port);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Module paramter view model " + "DeleteModule " + e);
            }
            
        }
        private void CloseModule()
        {
            try
            {
                Save?.Invoke(Port);
                this.HorizontalX = null;
                this.VerticalY = null;
                Reset();
                this.moduleStateManagement.resetParameterModule();
            }
            catch (Exception e)
            {
                MessageBox.Show("Module paramter view model " + "CloseModule " + e);
            }
            
        }
        private void ExecuteActiveHardware()
        {
            try
            {
                serviceProvider.GetRequiredService<IEnvironmentService>().ActiveHardware(Port);
            }
            catch (Exception e)
            {
                MessageBox.Show("Module paramter view model " + "ExecuteActiveHardware " + e);
            }
           
        }
        private void ExecuteReadConfigFromHardware()
        {
            try
            {
                var parameters = moduleStore.LoadParametersFromHardware(Port);
                string json = JsonConvert.SerializeObject(parameters);
                Dictionary<string, string> listParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                moduleStateManagement.readLoraConfigParameter(listParams);
            }
            catch (Exception e)
            {
                MessageBox.Show("Module paramter view model " + "ExecuteReadConfigFromHardware " + e);
            }
            
        }
        private void ExecuteConfigHardware()
        {
            try
            {
                var moduleObject = moduleStore.ModuleObjects.FirstOrDefault(x => x.id == Id);
                if (moduleObject != null)
                {
                    var tmp_moduleObject = new ModuleObject()
                    {
                        x = moduleObject.x,
                        y = moduleObject.y,
                        parameters = moduleObject.parameters,
                        id = moduleObject.id,
                        type = moduleObject.type,
                    };
                    tmp_moduleObject.mode = "MODE 3";
                    tmp_moduleObject.port = Port;
                    moduleStateManagement.updateParamsOfModule(tmp_moduleObject);
                }
                else
                {
                    var random = new Random();
                    var id = random.Next(50, 255);
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
            catch (Exception e)
            {
                MessageBox.Show("Module paramter view model " + "ExecuteConfigHardware " + e);
            }
            
        }
        private void OnConfigHardwareSuccess(ModuleObject moduleObject)
        {
            IsEnableSave = true;
            tmp_ModuleObject = moduleObject;
            /* var tmp_listPort = ListPort;
             tmp_listPort.Remove(portName);
             ListPort= tmp_listPort;*/
        }
        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentModuleViewModel));
        }

        private void OnUpdatePosition(object moduleObject)
        {
            try
            {
                string json = JsonConvert.SerializeObject(moduleObject);
                Dictionary<string, string> listParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                HorizontalX = listParams["x"];
                VerticalY = listParams["y"];
            }
            catch (Exception e)
            {
                MessageBox.Show("Module paramter view model " + "OnUpdatePosition " + e);
            }
            
        }
        private void OnIsActionUpdate(object isUpdate)
        {
            try
            {
                string json = JsonConvert.SerializeObject(isUpdate);
                Dictionary<string, string> listParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                IsUpdate = listParams["value"];
                IsEnableSave = false;
            }
            catch (Exception e)
            {
                MessageBox.Show("Module paramter view model " + "OnIsActionUpdate " + e);
            }
            
        }
        // Helper function ============
        public void Reset()
        {
            Id = null;
            Port = null;
            IsUpdate = null;
            ModuleType = null;
            tmp_ModuleObject = null;
            IsEnableSave = false;
            IsEnableDelete = false;
            IsEnablePortSelect = false;
        }
        public override void Dispose()
        {
            base.Dispose();
        }

    }
}
