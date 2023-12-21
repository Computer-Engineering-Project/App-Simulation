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
using Simulator1.Service;
using System.Windows.Threading;
using Environment.Model.VMParameter;
using System.Media;
using Simulator1.Database;
using System.Xml.Serialization;
using Environment;
using Simulator1.View;
using Simulator1.View.StatisticWindow;

namespace Simulator1.ViewModel
{
    public class MainViewModel : BaseViewModel, ICommunication
    {
        private ObservableCollection<ModuleObject> moduleObjects = new ObservableCollection<ModuleObject>();
        public ObservableCollection<ModuleObject> ModuleObjects { get => moduleObjects; set { moduleObjects = value; OnPropertyChanged(); } }

        private ObservableCollection<ButtonPort> ports;
        public ObservableCollection<ButtonPort> Ports { get => ports; set { ports = value; OnPropertyChanged(); } }

        private string clickedPort;

        private string programName;
        public string ProgramName { get => programName; set { programName = value; OnPropertyChanged(); } }

        /* private bool isEnableHistory = true;
         public bool IsEnableHistory { get => isEnableHistory; set { isEnableHistory = value; OnPropertyChanged(); } }*/

        private bool isEnableStop = false;
        public bool IsEnableStop { get => isEnableStop; set { isEnableStop = value; OnPropertyChanged(); } }

        private bool isEnablePause = false;
        public bool IsEnablePause { get => isEnablePause; set { isEnablePause = value; OnPropertyChanged(); } }

        private bool isEnableRun = true;
        public bool IsEnableRun { get => isEnableRun; set { isEnableRun = value; OnPropertyChanged(); } }

        private bool isDialogOpen = false;
        public bool IsDialogOpen { get => isDialogOpen; set { isDialogOpen = value; OnPropertyChanged(); } }

        //Animation
        private Visibility isLoadingPort = Visibility.Hidden;
        public Visibility IsLoadingPort { get => isLoadingPort; set { isLoadingPort = value; OnPropertyChanged(); } }

        //History Data
        private string informationAddress;
        public string InformationAddress { get => informationAddress; set { informationAddress = value; OnPropertyChanged(); } }

        private string sourceHistory;
        public string SourceHistory { get => sourceHistory; set { sourceHistory = value; OnPropertyChanged(); } }

        private string dataHistory;
        public string DataHistory { get => dataHistory; set { dataHistory = value; OnPropertyChanged(); } }

        private string delayTimeHistory;
        public string DelayTimeHistory { get => delayTimeHistory; set { delayTimeHistory = value; OnPropertyChanged(); } }

        private string distanceHistory;
        public string DistanceHistory { get => distanceHistory; set { distanceHistory = value; OnPropertyChanged(); } }

        private string rssiHistory;
        public string RSSIHistory { get => rssiHistory; set { rssiHistory = value; OnPropertyChanged(); } }

        private string snrHistory;
        public string SNRHistory { get => snrHistory; set { snrHistory = value; OnPropertyChanged(); } }

        private string typeErrorHistory;
        public string TypeErrorHistory { get => typeErrorHistory; set { typeErrorHistory = value; OnPropertyChanged(); } }

        private string timeHistory;
        public string TimeHistory { get => timeHistory; set { timeHistory = value; OnPropertyChanged(); } }

        private string lossProbalityHistory;
        public string LossProbalityHistory { get => lossProbalityHistory; set { lossProbalityHistory = value; OnPropertyChanged(); } }

        private string typeOfNoise = "SNR from dB";
        public string TypeOfNoise { get => typeOfNoise; set { typeOfNoise = value; OnPropertyChanged(); } }

        private string unitOfNoise = "dB";
        public string UnitOfNoise { get => unitOfNoise; set { unitOfNoise = value; OnPropertyChanged(); } }

        private string noise;
        public string Noise { get => noise; set { noise = value; OnPropertyChanged(); } }

        private HistoryObject selectedItemHistory;
        public HistoryObject SelectedItemHistory { get => selectedItemHistory; set { selectedItemHistory = value; OnPropertyChanged(); } }

        private ObservableCollection<HistoryObject> historyObjectIns;
        public ObservableCollection<HistoryObject> HistoryObjectIns { get => historyObjectIns; set { historyObjectIns = value; OnPropertyChanged(); } }

        private ObservableCollection<HistoryObject> historyObjectOuts;
        public ObservableCollection<HistoryObject> HistoryObjectOuts { get => historyObjectOuts; set { historyObjectOuts = value; OnPropertyChanged(); } }

        private ObservableCollection<HistoryObject> historyObjectErrors;
        public ObservableCollection<HistoryObject> HistoryObjectErrors { get => historyObjectErrors; set { historyObjectErrors = value; OnPropertyChanged(); } }

        private readonly MainViewStore mainStore;
        private readonly ModuleStore moduleStore;
        private readonly ModuleStateManagement moduleStateManagement;
        private readonly IServiceProvider serviceProvider;
        private readonly MainStateManagement mainStateManagement;
        private readonly HistoryDataStore historyDataStore;
        private readonly StatusStateManagement statusStateManagement;

        public BaseViewModel CurrentModuleViewModel => mainStore.CurrentViewModel;
        /*public ModuleParameterViewModel ModuleParameterViewModel { get => moduleParameterViewModel; set { moduleParameterViewModel = value; OnPropertyChanged(); } }*/
        /*private ModuleParameterStore moduleParameterStore = new ModuleParameterStore();*/
        /*public ModuleParameterStore ModuleParameterStore { get => moduleParameterStore; set { moduleParameterStore = value; OnPropertyChanged(); } }*/
        public ICommand OpenDialogCommand { get; set; }
        public ICommand UpdateModuleCommand { get; set; }
        public ICommand LoadHistoryFileCommand { get; set; }
        public ICommand SaveHistoryCommmand { get; set; }
        public ICommand SaveAsHistoryCommmand { get; set; }
        public ICommand RunEnvironmentCommand { get; set; }
        public ICommand PauseEnvironmentCommand { get; set; }
        public ICommand StopEnvironmentCommand { get; set; }
        public ICommand LoadPorts { get; set; }

        public ICommand testCommand { get; set; }
        public ICommand autoSaveCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }
        public ICommand ClearHistoryDataCommand { get; set; }

        public ICommand EnvdBSNRCommand { get; set; }
        public ICommand EnvPowerSNRCommand { get; set; }
        public ICommand EnvVoltageSNRCommand { get; set; }
        public ICommand ConfigENVNoise { get; set; }

        ~MainViewModel() { }
        public MainViewModel(MainViewStore mainStore, MainStateManagement mainStateManagement, ModuleStateManagement moduleStateManagement,
            ModuleStore moduleStore, IServiceProvider serviceProvider, HistoryDataStore historyDataStore,
            StatusStateManagement statusStateManagement)
        {

            //DI
            this.mainStore = mainStore;
            this.moduleStore = moduleStore;
            this.moduleStateManagement = moduleStateManagement;
            this.serviceProvider = serviceProvider;
            this.mainStateManagement = mainStateManagement;
            this.historyDataStore = historyDataStore;
            this.statusStateManagement = statusStateManagement;

            //Variable
            Ports = new ObservableCollection<ButtonPort>();
            HistoryObjectIns = new ObservableCollection<HistoryObject>();
            HistoryObjectOuts = new ObservableCollection<HistoryObject>();
            historyObjectErrors = new ObservableCollection<HistoryObject>();
            ProgramName = "Software Simulator";

            //Event delegate
            this.moduleStateManagement.ModuleObjectCreated += OnModuleObjectCreated;
            this.moduleStateManagement.ChangePositionAndPort += ExecuteAutoSavePosition;
            this.moduleStateManagement.DeleteModule += OnDeleteModule;
            this.mainStateManagement.IsRunningNow += OnIsRunningNow;
            this.mainStateManagement.IsIdleNow += OnIsIdleNow;
            this.mainStateManagement.IsPauseNow += OnIsPauseNow;
            this.mainStateManagement.UpdateHistoryOut += OnUpdateHistoryOut;
            this.mainStateManagement.UpdateHitoryIn += OnUpdateHistoryIn;
            this.mainStateManagement.UpdateHistoryError += OnUpdateHistoryError;
            this.mainStateManagement.ResetAll += OnReset;
            this.mainStateManagement.ChangeMode += OnChangeMode;

            this.statusStateManagement.StatusChanged += OnStatusChanged;

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
            LoadHistoryFileCommand = new RelayCommand(() => ExecuteLoadHistoryFile());
            SaveHistoryCommmand = new RelayCommand(() => ExecuteSaveHistory());
            SaveAsHistoryCommmand = new RelayCommand(() => ExecuteSaveAsHistory());
            autoSaveCommand = new ParameterRelayCommand<object>((o) => { return true; }, (o) =>
            {
                ExecuteAutoSavePosition(o);
            });
            SelectionChangedCommand = new RelayCommand(() => DectectActionSelectedColumn());
            ClearHistoryDataCommand = new RelayCommand(() => ExecuteClearHistoryData());

            RunEnvironmentCommand = new RelayCommand(() => ExecuteRunEnvironment());
            StopEnvironmentCommand = new RelayCommand(() => ExecuteStopEnvironment());
            PauseEnvironmentCommand = new RelayCommand(() => ExecutePauseEnvironment());
            LoadPorts = new RelayCommand(() => ExecuteLoadPorts());
            EnvdBSNRCommand = new RelayCommand(() => ExecuteChangeTypeOfNoise("dB"));
            EnvPowerSNRCommand = new RelayCommand(() => ExecuteChangeTypeOfNoise("W"));
            EnvVoltageSNRCommand = new RelayCommand(() => ExecuteChangeTypeOfNoise("V"));
            ConfigENVNoise = new RelayCommand(() => ExecuteConfigENVNoise());
        }
        //Delegate handler
        private void OnStatusChanged()
        {
            if (!ProgramName.Contains('*'))
            {
                var nameUnSave = ProgramName + "*";
                ProgramName = nameUnSave;
            }
        }
        private void OnIsRunningNow()
        {
            IsEnableRun = false;
            IsEnablePause = true;
            IsEnableStop = true;
        }
        private void OnIsIdleNow()
        {
            IsEnableRun = true;
            IsEnablePause = false;
            IsEnableStop = false;
        }
        private void OnIsPauseNow()
        {
            IsEnableRun = true;
            IsEnablePause = false;
            IsEnableStop = true;
        }
        private void OnChangeMode(object data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data);
                Dictionary<string, string> listParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                foreach (var module in moduleStore.ModuleObjects)
                {
                    if (listParams["id"] != null)
                    {
                        if (module.id == listParams["id"])
                        {
                            module.mode = "MODE " + listParams["mode"];
                        }
                    }
                }
                ModuleObjects = new ObservableCollection<ModuleObject>(moduleStore.ModuleObjects);
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "OnChangeMode " + e);
            }
        }
        private void OnUpdateHistoryOut(string portName)
        {
            try
            {
                var moduleHistory = historyDataStore.ModuleHistories.FirstOrDefault(x => x.moduleObject.port == portName);
                if (moduleHistory != null)
                {
                    HistoryObjectOuts = new ObservableCollection<HistoryObject>(moduleHistory.UI_historyObjectOuts);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "OnUpdateHistoryOut " + e);
            }

        }
        private void OnUpdateHistoryIn(string portName)
        {
            try
            {
                var moduleHistory = historyDataStore.ModuleHistories.FirstOrDefault(x => x.moduleObject.port == portName);
                if (moduleHistory != null)
                {
                    HistoryObjectIns = new ObservableCollection<HistoryObject>(moduleHistory.UI_historyObjectIns);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "OnUpdateHistoryIn " + e);
            }
        }
        private void OnUpdateHistoryError(string portName)
        {
            try
            {
                var moduleHistory = historyDataStore.ModuleHistories.FirstOrDefault(x => x.moduleObject.port == portName);
                if (moduleHistory != null)
                {
                    HistoryObjectErrors = new ObservableCollection<HistoryObject>(moduleHistory.UI_historyObjectErrors);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "OnUpdateHistoryError " + e);
            }
        }
        private void OnModuleObjectCreated(string portName)
        {
            try
            {
                ModuleObjects = new ObservableCollection<ModuleObject>(moduleStore.ModuleObjects);
                var ports = Ports;
                foreach (var p in ports)
                {
                    if (p.PortName == portName)
                    {
                        p.Color = "LightGreen";
                    }
                }
                Ports = new ObservableCollection<ButtonPort>(ports);
                /*IsEnableHistory = false;*/
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "OnModuleObjectCreated " + e);
            }

        }
        private void OnDeleteModule(string portName)
        {
            try
            {
                var tmp_ports = Ports;
                var p = tmp_ports.FirstOrDefault(x => x.PortName == portName);
                if (p != null)
                {
                    p.Color = "Wheat";
                }
                Ports = tmp_ports;
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "OnDeleteModule " + e);
            }

        }
        public void OnReset()
        {
            Reset();
        }
        //Command handler
        private void ExecuteClearHistoryData()
        {
            var moduleHistory = historyDataStore.ModuleHistories.FirstOrDefault(x => x.moduleObject.port == clickedPort);
            if (moduleHistory != null)
            {
                moduleHistory.UI_historyObjectErrors.Clear();
                moduleHistory.UI_historyObjectIns.Clear();
                moduleHistory.UI_historyObjectOuts.Clear();
                HistoryObjectIns = new ObservableCollection<HistoryObject>();
                HistoryObjectOuts = new ObservableCollection<HistoryObject>();
                HistoryObjectErrors = new ObservableCollection<HistoryObject>();
            }

        }
        private void ExecuteConfigENVNoise()
        {
            serviceProvider.GetRequiredService<IEnvironmentService>().setNoise(Noise);
            MessageBox.Show("Config noise success");
        }
        private void ExecuteChangeTypeOfNoise(string typeOfNoise)
        {
            UnitOfNoise = typeOfNoise;
            if (typeOfNoise == "W")
            {
                TypeOfNoise = "Power SNR";
            }
            else if (typeOfNoise == "V")
            {
                TypeOfNoise = "Voltage SNR";
            }
            else
            {
                TypeOfNoise = "SNR from dB";
            }
        }
        private void ExecuteSaveHistory()
        {
            try
            {
                if (moduleStore.SaveHistoryToJsonFile("save"))
                {
                    ProgramName = "Software Simulator";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "ExecuteSaveHistory " + e);
            }
        }
        private void ExecuteSaveAsHistory()
        {
            try
            {
                if (moduleStore.SaveHistoryToJsonFile("saveas"))
                {
                    ProgramName = "Software Simulator";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "ExecuteSaveAsHistory " + e);
            }
        }
        private void ExecuteAutoSavePosition(object positionObject)
        {
            try
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
                            var transX = Double.Parse(listParams["transformX"]);
                            module.transformX = transX;
                            var transY = Double.Parse(listParams["transformY"]);
                            module.transformY = transY;
                        }
                    }
                    else
                    {
                        if (module.port == listParams["port"])
                        {
                            var x = Double.Parse(listParams["x"]);
                            if (x < 0) x = 0;
                            module.x = x;
                            module.transformX = x / 10 + 20 - module.coveringLossDiameter / 2;
                            var y = Double.Parse(listParams["y"]);
                            if (y < 0) y = 0;
                            module.y = y;
                            module.transformY = y / 10 + 20 - module.coveringLossDiameter / 2;

                        }
                    }

                }

                ModuleObjects = new ObservableCollection<ModuleObject>(moduleStore.ModuleObjects);
                statusStateManagement.statusChanged();
                serviceProvider.GetRequiredService<IEnvironmentService>().changeModuleObjectsPosition(new List<ModuleObject>(ModuleObjects));
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "ExecuteAutoSavePosition " + e);
            }

        }
        private void ExecuteClickPort(string portName)
        {
            try
            {
                serviceProvider.GetRequiredService<IEnvironmentService>().passPortClicked(portName);
                var portObject = Ports.FirstOrDefault(x => x.PortName == portName);
                if (portObject != null)
                {
                    if (portObject.Color == "Wheat")
                    {
                        OpenDialog(portName);
                    }
                    else
                    {
                        //Binding history data to history table
                        clickedPort = portName;
                        var moduleHistory = historyDataStore.ModuleHistories.FirstOrDefault(x => x.moduleObject.port == portName);
                        if (moduleHistory != null)
                        {
                            string json = JsonConvert.SerializeObject(moduleHistory.moduleObject.parameters);
                            Dictionary<string, string> listParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                            if (moduleHistory.moduleObject.type == ModuleObjectType.LORA)
                            {
                                InformationAddress = "0x" + listParams["Address"] + " --- " + "0x" + listParams["Channel"] + " --- " + portName;
                            }
                            else
                            {
                                InformationAddress = "0x" + listParams["Address"] + " --- " + listParams["Channel"] + " --- " + portName;
                            }

                            HistoryObjectIns = new ObservableCollection<HistoryObject>(moduleHistory.UI_historyObjectIns);
                            HistoryObjectOuts = new ObservableCollection<HistoryObject>(moduleHistory.UI_historyObjectOuts);
                            HistoryObjectErrors = new ObservableCollection<HistoryObject>(moduleHistory.UI_historyObjectErrors);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "ExecuteClickPort " + e);
            }

        }
        private void OpenDialog(string portName)
        {
            try
            {
                IsDialogOpen = true;
                var tmp_Ports = Ports.Select(x => x.PortName).ToList();
                foreach (var port in Ports)
                {
                    if (port.Color == "LightGreen")
                    {
                        tmp_Ports.Remove(port.PortName);
                    }
                }
                if (CurrentModuleViewModel is ModuleParameterViewModel)
                {
                    serviceProvider.GetRequiredService<IEnvironmentService>().startPort(portName);
                    ((ModuleParameterViewModel)CurrentModuleViewModel).Save += CloseDialog;
                    moduleStateManagement.updateModuleVMParams(new ModuleParameterVM()
                    {
                        port = portName,
                        listPort = tmp_Ports,
                        isEnableDelete = false,
                        isEnablePortSelect = false,
                        kindOfModule = "",
                        moduleType = ""
                    });
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "OpenDialog " + e);
            }

        }
        private void ExecuteOpenUpdateModule(ModuleObject module)
        {
            try
            {
                if (!IsEnableRun)
                {
                    return;
                }
                IsDialogOpen = true;
                var tmp_Ports = Ports.Select(x => x.PortName).ToList();
                foreach (var port in Ports)
                {
                    if (port.Color == "LightGreen")
                    {
                        tmp_Ports.Remove(port.PortName);
                    }
                }
                if (CurrentModuleViewModel is ModuleParameterViewModel)
                {
                    var moduleParameterVM = new ModuleParameterVM()
                    {
                        id = module.id,
                        isEnableDelete = true,
                        listPort = tmp_Ports,
                        isUpdate = "true",
                        moduleType = module.type,
                        kindOfModule = module.kind
                    };

                    if (module.port != null)
                    {
                        serviceProvider.GetRequiredService<IEnvironmentService>().startPort(module.port);
                        moduleParameterVM.port = module.port;
                        moduleParameterVM.isEnablePortSelect = false;
                    }
                    else
                    {
                        moduleParameterVM.isEnablePortSelect = true;
                    }
                    ((ModuleParameterViewModel)CurrentModuleViewModel).Save += CloseDialog;
                    moduleStateManagement.updateModuleVMParams(moduleParameterVM);
                }
                if (module.parameters != null)
                {
                    string jsonParameters = JsonConvert.SerializeObject(module.parameters);
                    if (module.type == ModuleObjectType.LORA)
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
                    else if (module.type == ModuleObjectType.ZIGBEE)
                    {
                        var zigbeeParameters = JsonConvert.DeserializeObject<ZigbeeParameterObject>(jsonParameters);
                        if (CurrentModuleViewModel is ModuleParameterViewModel)
                        {
                            moduleStateManagement.openUpdateZigbeeParameter(zigbeeParameters);
                            moduleStateManagement.updatePosition(new
                            {
                                x = module.x,
                                y = module.y,
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "ExecuteOpenUpdateModule " + e);
            }

        }
        private void ExecuteLoadHistoryFile()
        {
            try
            {
                mainStateManagement.loadHistoryFromDB();
                ModuleObjects = new ObservableCollection<ModuleObject>(moduleStore.ModuleObjects);
                /* IsEnableHistory = false;*/
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "ExecuteLoadHistory " + e);
            }

        }

        private void CloseDialog(string port)
        {
            try
            {
                IsDialogOpen = false;
                ModuleObjects = new ObservableCollection<ModuleObject>(moduleStore.ModuleObjects);
                if (port != null)
                {
                    serviceProvider.GetRequiredService<IEnvironmentService>().closePort(port);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "CloseDialog " + e);
            }

        }
        private void ExecuteLoadPorts()
        {
            IsLoadingPort = Visibility.Visible;
            try
            {
                var ports = serviceProvider.GetRequiredService<IEnvironmentService>().loadPorts();
                var previousPorts = Ports.Select(x => x.PortName).ToList();
                var tmpPorts = Ports;
                if (ports != null)
                {
                    var addPorts = ports.Except(previousPorts);
                    var removePorts = previousPorts.Except(ports);
                    foreach (var port in addPorts)
                    {
                        tmpPorts.Add(new ButtonPort()
                        {
                            Color = "Wheat",
                            PortName = port
                        });
                        moduleStore.Ports.Add(port);
                    }
                    foreach (var port in removePorts)
                    {
                        var _object = tmpPorts.Where(x => x.PortName == port).FirstOrDefault();
                        if (_object != null)
                        {
                            tmpPorts.Remove(_object);
                            moduleStore.Ports.Remove(port);
                        }
                    }
                    Ports = tmpPorts;

                }
                IsLoadingPort = Visibility.Hidden;
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "ExecuteLoadPorts " + e);
            }

        }

        private void DectectActionSelectedColumn()
        {
            try
            {
                if (SelectedItemHistory != null)
                {
                    SourceHistory = SelectedItemHistory.Source;
                    DataHistory = SelectedItemHistory.Data;
                    DelayTimeHistory = SelectedItemHistory.DelayTime == null ? "Na/N" : SelectedItemHistory.DelayTime;
                    DistanceHistory = SelectedItemHistory.Distance == null ? "Na/N" : SelectedItemHistory.Distance;
                    RSSIHistory = SelectedItemHistory.RSSI == null ? "Na/N" : SelectedItemHistory.RSSI;
                    SNRHistory = SelectedItemHistory.SNR == null ? "Na/N" : SelectedItemHistory.SNR;
                    TimeHistory = SelectedItemHistory.Time == null ? "Na/N" : SelectedItemHistory.Time;
                    if (SelectedItemHistory.TypeError == ERROR_TYPE.OUT_OF_RANGE)
                    {
                        TypeErrorHistory = "Out of range";
                    }
                    else if (SelectedItemHistory.TypeError == ERROR_TYPE.PATH_LOSS)
                    {
                        TypeErrorHistory = "Path loss";
                    }
                    else if (SelectedItemHistory.TypeError == ERROR_TYPE.COLLIDED)
                    {
                        TypeErrorHistory = "Collided";
                    }
                    else
                    {
                        TypeErrorHistory = "Na/N";
                    }
                    LossProbalityHistory = SelectedItemHistory.LossProbality == null ? "Na/N" : SelectedItemHistory.LossProbality;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "DectectActionSelectedColumn " + e);
            }
            //todo

        }
        // Clear all history data
        private void ClearHistoryData()
        {

        }
        //Reset all
        private void Reset()
        {
            try
            {
                var portReset = Ports.Select(x => new ButtonPort()
                {
                    PortName = x.PortName,
                    Color = "Wheat"
                }).ToList();
                Ports = new ObservableCollection<ButtonPort>(portReset);
                ModuleObjects.Clear();
                moduleStore.Reset();
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "Reset " + e);
            }
        }
        //Dispose
        public override void Dispose()
        {
            try
            {
                moduleStateManagement.ModuleObjectCreated -= OnModuleObjectCreated;
                moduleStateManagement.ChangePositionAndPort -= ExecuteAutoSavePosition;
                moduleStateManagement.DeleteModule -= OnDeleteModule;
                mainStateManagement.IsRunningNow -= OnIsRunningNow;
                mainStateManagement.IsIdleNow -= OnIsIdleNow;
                mainStateManagement.IsPauseNow -= OnIsPauseNow;
                mainStateManagement.UpdateHistoryOut -= OnUpdateHistoryOut;
                mainStateManagement.UpdateHitoryIn -= OnUpdateHistoryIn;
                mainStateManagement.UpdateHistoryError -= OnUpdateHistoryError;
                base.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "Dispose " + e);
            }

        }
        //Invoke to Environment
        private void ExecuteRunEnvironment()
        {
            try
            {
                if (!checkIsAvailableToRun())
                {
                    return;
                }
                serviceProvider.GetRequiredService<IEnvironmentService>().passModuleObjects(new List<ModuleObject>(ModuleObjects));
                serviceProvider.GetRequiredService<IEnvironmentService>().Run();
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "ExecuteRunEnvironment " + e);
            }
        }
        private bool checkIsAvailableToRun()
        {
            if (ModuleObjects.Count == 0)
            {
                return false;
            }
            foreach (var m in ModuleObjects)
            {
                if (string.IsNullOrEmpty(m.port))
                {
                    MessageBox.Show("One of these modules doesn't have port to connect");
                    return false;
                }
            }
            return true;
        }
        private void ExecutePauseEnvironment()
        {
            try
            {
                serviceProvider.GetRequiredService<IEnvironmentService>().Pause();
                serviceProvider.GetRequiredService<IEnvironmentService>().closeThreads();
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "ExecutePauseEnvironment " + e);
            }

        }
        private void ExecuteStopEnvironment()
        {
            try
            {
                serviceProvider.GetRequiredService<IEnvironmentService>().Stop();
                historyDataStore.ClearHistoryData();
                HistoryObjectIns = new ObservableCollection<HistoryObject>();
                HistoryObjectOuts = new ObservableCollection<HistoryObject>();

                ChartWindow chartWindow = serviceProvider.GetRequiredService<ChartWindow>();
                chartWindow.Show();
                mainStateManagement.openChart();

            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "ExecuteStopEnvironment " + e);
            }

        }
        //Received Data from Environment
        public void showQueueReceivedFromHardware(PacketSendTransferToView transferedPacket, string portClicked)
        {
            try
            {
                var moduleHistory = historyDataStore.ModuleHistories.FirstOrDefault(x => x.moduleObject.port == transferedPacket.portName);
                if (moduleHistory != null)
                {
                    var newHistoryObject = new HistoryObject();
                    var length = moduleHistory.UI_historyObjectOuts.Count;
                    newHistoryObject.Id = length + 1;
                    if (moduleHistory.moduleObject.type == ModuleObjectType.LORA)
                    {
                        var loraParams = (LoraParameterObject)moduleHistory.moduleObject.parameters;
                        if (string.IsNullOrEmpty(transferedPacket.packet.address))
                        {
                            transferedPacket.packet.address = loraParams.Address;
                            transferedPacket.packet.channel = loraParams.Channel;
                        }
                        newHistoryObject.Source = "Address: 0x" + transferedPacket.packet.address + "--- Channel: 0x" + loraParams.Channel;

                    }
                    else if (moduleHistory.moduleObject.type == ModuleObjectType.ZIGBEE)
                    {
                        var zigbeeParams = (ZigbeeParameterObject)moduleHistory.moduleObject.parameters;
                        newHistoryObject.Source = "Address: " + zigbeeParams.Address + "--- Channel: " + zigbeeParams.Channel;
                    }
                    newHistoryObject.Data = transferedPacket.packet.data;
                    moduleHistory.UI_historyObjectOuts.Enqueue(newHistoryObject);
                    moduleHistory.historyObjectOuts.Enqueue(newHistoryObject);
                    mainStateManagement.updateHistoryOut(portClicked);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "showQueueReceivedFromHardware " + e);
            }

        }
        public void showQueueReceivedFromOtherDevice(PacketReceivedTransferToView transferedPacket, string portClicked)
        {
            try
            {
                var moduleHistory = historyDataStore.ModuleHistories.FirstOrDefault(x => x.moduleObject.port == transferedPacket.packet.receivedModule.port);
                if (moduleHistory != null)
                {
                    var newHistoryObject = new HistoryObject();
                    if (moduleHistory.moduleObject.type == ModuleObjectType.LORA)
                    {
                        var loraParams = (LoraParameterObject)transferedPacket.packet.sourceModule.parameters;
                        if (loraParams != null)
                        {
                            newHistoryObject = new HistoryObject()
                            {
                                Time = transferedPacket.packet.timeUTC,
                                Source = "Address: 0x" + loraParams.Address + "--- Channel: 0x" + loraParams.Channel,
                            };
                        }
                    }
                    else if (moduleHistory.moduleObject.type == ModuleObjectType.ZIGBEE)
                    {
                        var zigbeeParams = (ZigbeeParameterObject)transferedPacket.packet.sourceModule.parameters;
                        if (zigbeeParams != null)
                        {
                            newHistoryObject = new HistoryObject()
                            {
                                Time = transferedPacket.packet.timeUTC,
                                Source = "Address: " + zigbeeParams.Address + "--- Channel: " + zigbeeParams.Channel,
                            };
                        }
                    }
                    newHistoryObject.Data = transferedPacket.packet.packet.data;
                    newHistoryObject.DelayTime = transferedPacket.packet.DelayTime.ToString("F3");
                    newHistoryObject.Distance = transferedPacket.packet.Distance;
                    newHistoryObject.RSSI = transferedPacket.packet.RSSI;
                    newHistoryObject.SNR = transferedPacket.packet.SNR;
                    newHistoryObject.LossProbality = transferedPacket.packet.lossProbality;
                    moduleHistory.UI_historyObjectIns.Enqueue(newHistoryObject);
                    moduleHistory.historyObjectIns.Enqueue(newHistoryObject);
                    mainStateManagement.updateHistoryIn(portClicked);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "showQueueReceivedFromOtherDevice " + e);
            }

        }
        public void showQueueError(PacketReceivedTransferToView errorPacket, string portClicked)
        {
            try
            {
                var moduleHistory = historyDataStore.ModuleHistories.FirstOrDefault(x => x.moduleObject.port == errorPacket.packet.receivedModule.port);
                if (moduleHistory != null)
                {
                    var errorHisObj = new HistoryObject();
                    if (moduleHistory.moduleObject.type == ModuleObjectType.LORA)
                    {
                        var paras = (LoraParameterObject)errorPacket.packet.sourceModule.parameters;
                        if (paras != null)
                        {
                            errorHisObj.Time = errorPacket.packet.timeUTC;
                            errorHisObj.Source = "Address: 0x" + paras.Address + "--- Channel: 0x" + paras.Channel;
                            errorHisObj.TypeError = errorPacket.packet.typeError;
                            errorHisObj.Data = errorPacket.packet.packet.data;
                            errorHisObj.DelayTime = errorPacket.packet.DelayTime.ToString("F3");
                            errorHisObj.Distance = errorPacket.packet.Distance;
                            errorHisObj.RSSI = errorPacket.packet.RSSI;
                            errorHisObj.SNR = errorPacket.packet.SNR;
                            errorHisObj.LossProbality = errorPacket.packet.lossProbality;
                        }
                    }
                    else
                    {
                        var paras = (ZigbeeParameterObject)errorPacket.packet.sourceModule.parameters;
                        if (paras != null)
                        {
                            errorHisObj.Time = errorPacket.packet.timeUTC;
                            errorHisObj.Source = "Address: " + paras.Address + "--- Channel: " + paras.Channel;
                            errorHisObj.TypeError = errorPacket.packet.typeError;
                            errorHisObj.Data = errorPacket.packet.packet.data;
                            errorHisObj.DelayTime = errorPacket.packet.DelayTime.ToString("F3");
                            errorHisObj.Distance = errorPacket.packet.Distance;
                            errorHisObj.RSSI = errorPacket.packet.RSSI;
                            errorHisObj.SNR = errorPacket.packet.SNR;
                            errorHisObj.LossProbality = errorPacket.packet.lossProbality;
                        }
                    }
                    moduleHistory.UI_historyObjectErrors.Enqueue(errorHisObj);
                    moduleHistory.historyObjectErrors.Enqueue(errorHisObj);
                    mainStateManagement.updateHistoryError(portClicked);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "showQueueError " + e);
            }
        }
        public void deviceChangeMode(int mode, string id)
        {
            try
            {
                mainStateManagement.changeMode(new
                {
                    mode = mode,
                    id = id
                });
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "deviceChangeMode " + e);
            }
        }

        public void sendMessageIsRunning()
        {
            mainStateManagement.isRunningNow();
        }

        public void sendMessageIsPause()
        {
            mainStateManagement.isPauseNow();

        }

        public void sendMessageIsStop()
        {
            mainStateManagement.isIdleNow();
        }


    }
}
