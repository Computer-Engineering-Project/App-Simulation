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
        public string HorizontalX { get => horizontal_x; set { horizontal_x = value; OnPropertyChanged(); } }

        private string vertical_y;
        public string VerticalY { get => vertical_y; set { vertical_y = value; OnPropertyChanged(); } }

        private ObservableCollection<string> listPort;
        public ObservableCollection<string> ListPort { get => listPort; set { listPort = value; OnPropertyChanged(); } }

        private string port;
        public string Port { get => port; set { port = value; OnPropertyChanged(); } }

        private string id;
        public string Id { get => id; set { id = value; OnPropertyChanged(); } }

        private string isUpdate;
        public string IsUpdate { get => isUpdate; set { isUpdate = value; OnPropertyChanged(); } }

        private string moduleType;
        public string ModuleType { get => moduleType; set { moduleType = value; OnPropertyChanged(); } }

        private string kindOfModule;
        public string KindOfModule { get => kindOfModule; set { kindOfModule = value; OnPropertyChanged(); } }

        private ModuleObject tmp_moduleObject;
        public ModuleObject tmp_ModuleObject { get => tmp_moduleObject; set { tmp_moduleObject = value; OnPropertyChanged(); } }

        private bool isEnableSave = false;
        public bool IsEnableSave { get => isEnableSave; set { isEnableSave = value; OnPropertyChanged(); } }

        private bool isEnableDelete = false;
        public bool IsEnableDelete { get => isEnableDelete; set { isEnableDelete = value; OnPropertyChanged(); } }

        private bool isEnablePortSelect = false;
        public bool IsEnablePortSelect { get => isEnablePortSelect; set { isEnablePortSelect = value; OnPropertyChanged(); } }

        private ObservableCollection<string> listModuleType = new ObservableCollection<string>();
        public ObservableCollection<string> ListModuleType { get => listModuleType; set { listModuleType = value; OnPropertyChanged(); } }
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
                    KindOfModule = moduleParameterVM.kindOfModule;
                    tmp_ModuleObject = moduleParameterVM.tmp_moduleObject;
                    IsEnableSave = moduleParameterVM.isEnableSave;
                    IsEnableDelete = moduleParameterVM.isEnableDelete;
                    IsEnablePortSelect = moduleParameterVM.isEnablePortSelect;
                    if (ModuleType == "lora")
                    {
                        ((NavigateCommand)LoraParamCommand).Execute(new { });
                    }
                    else if (ModuleType == "zigbee")
                    {
                        ((NavigateCommand)ZigbeeParamCommand).Execute(new { });
                    }
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
            if (type == "lora")
            {
                ListModuleType = new ObservableCollection<string>() { "E32-433T20D" };
            }
            else
            {
                ListModuleType = new ObservableCollection<string>() { "DL-22" };
            }
        }
        private void SelectKindOfModule()
        {
            if (string.IsNullOrEmpty(ModuleType))
            {
                ExecuteChangeModuleType("lora");
            }
            if (ModuleType == "lora")
            {
                if (string.IsNullOrEmpty(KindOfModule))
                    KindOfModule = "E32-433T20D";
            }
            else
            {
                if (string.IsNullOrEmpty(KindOfModule))
                    KindOfModule = "DL-22";
            }
        }
        private void GenerateModule()
        {
            try
            {
                var random = new Random();
                if (string.IsNullOrEmpty(HorizontalX))
                {
                    HorizontalX = (random.Next(501) * 10).ToString();
                }
                if (string.IsNullOrEmpty(VerticalY))
                {
                    VerticalY = (random.Next(501) * 10).ToString();
                }
                if (IsUpdate != "true")
                {
                    var x = Double.Parse(HorizontalX);
                    if (x < 0) x = 0;
                    var y = Double.Parse(VerticalY);
                    if (y < 0) y = 0;
                    tmp_ModuleObject.x = x;
                    tmp_ModuleObject.transformX = x / 10 - tmp_moduleObject.coveringAreaDiameter / 2 + 20;
                    tmp_ModuleObject.y = y;
                    tmp_ModuleObject.transformY = y / 10 - tmp_moduleObject.coveringAreaDiameter / 2 + 20;
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
                            m.coveringAreaRange = tmp_ModuleObject.coveringAreaRange;
                            m.coveringAreaDiameter = tmp_ModuleObject.coveringAreaRange / 5;
                            m.transformX = tmp_ModuleObject.x / 10 - tmp_moduleObject.coveringAreaDiameter / 2 + 20;
                            m.transformY = tmp_ModuleObject.y / 10 - tmp_moduleObject.coveringAreaDiameter / 2 + 20;
                            m.mode = tmp_ModuleObject.mode;
                            m.parameters = tmp_ModuleObject.parameters;
                            m.type = tmp_ModuleObject.type;
                            m.kind = tmp_ModuleObject.kind;
                            historyStateManagement.reloadHistoryData(m);
                        }
                    }
                    moduleStateManagement.changePositionAndPort(module);
                    moduleStateManagement.createModuleObject(module.port);
                }
                statusStateManagement.statusChanged();
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
                statusStateManagement.statusChanged();
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
                if (listParams == null)
                {
                    MessageBox.Show("Please choose file has module information");
                    return;
                }
                if (ModuleType == ModuleObjectType.LORA)
                {
                    moduleStateManagement.readLoraConfigParameter(listParams);
                    IsEnableSave = true;
                }
                else if (ModuleType == ModuleObjectType.ZIGBEE)
                {
                    moduleStateManagement.readZigbeeConfigParameter(listParams);
                    IsEnableSave = true;
                }
                else
                {
                    MessageBox.Show("Please choose type module before read config");
                }

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
                SelectKindOfModule();

                var moduleObject = moduleStore.ModuleObjects.FirstOrDefault(x => x.id == Id);
                if (moduleObject != null)
                {
                    var tmp_moduleObject = new ModuleObject();


                    tmp_moduleObject.x = moduleObject.x;
                    tmp_moduleObject.y = moduleObject.y;
                    tmp_moduleObject.parameters = moduleObject.parameters;
                    tmp_moduleObject.id = moduleObject.id;
                    tmp_moduleObject.type = ModuleType;
                    tmp_moduleObject.kind = KindOfModule;
                    tmp_moduleObject.port = Port;
                    if (moduleObject.type == "lora")
                    {
                        tmp_moduleObject.mode = "MODE 3";
                        moduleStateManagement.updateLoraParamsOfModule(tmp_moduleObject);
                    }
                    else if (moduleObject.type == "zigbee")
                    {
                        moduleStateManagement.updateZigbeeParamsOfModule(tmp_moduleObject);
                    }

                }
                else
                {
                    var random = new Random();
                    var id = random.Next(50, 255);
                    var module = new ModuleObject();

                    module.port = Port;
                    module.id = id.ToString();
                    module.type = ModuleType;
                    module.kind = KindOfModule;
                    if (ModuleType == "lora")
                    {
                        module.mode = "MODE 3";
                        moduleStateManagement.createLoraParameter(module);
                    }
                    else if (ModuleType == "zigbee")
                    {
                        moduleStateManagement.createZigbeeParameter(module);
                    }
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
            this.moduleParamViewStore.CurrentModuleViewModelChanged -= OnCurrentViewModelChanged;
            this.moduleStateManagement.UpdatePosition -= OnUpdatePosition;
            this.moduleStateManagement.IsActionUpdate -= OnIsActionUpdate;
            this.moduleStateManagement.ConfigHardwareSuccess -= OnConfigHardwareSuccess;
            this.moduleStateManagement.UpdateParamsForModuleParameterVM -= OnUpdateParamterForVM;
            base.Dispose();
        }

    }
}
