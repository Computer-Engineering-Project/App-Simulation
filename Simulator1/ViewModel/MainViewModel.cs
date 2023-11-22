﻿using System;
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

namespace Simulator1.ViewModel
{
    public class MainViewModel : BaseViewModel, ICommunication
    {
        private ObservableCollection<ModuleObject> moduleObjects;
        public ObservableCollection<ModuleObject> ModuleObjects { get => moduleObjects; set { moduleObjects = value; OnPropertyChanged(); } }

        private ObservableCollection<ButtonPort> ports;
        public ObservableCollection<ButtonPort> Ports { get => ports; set { ports = value; OnPropertyChanged(); } }

        private bool isEnableHistory = true;
        public bool IsEnableHistory { get => isEnableHistory; set { isEnableHistory = value; OnPropertyChanged(); } }

        private bool isEnableStop = false;
        public bool IsEnableStop { get => isEnableStop; set { isEnableStop = value; OnPropertyChanged(); } }

        private bool isEnablePause = false;
        public bool IsEnablePause { get => isEnablePause; set { isEnablePause = value; OnPropertyChanged(); } }

        private bool isEnableRun = true;
        public bool IsEnableRun { get => isEnableRun; set { isEnableRun = value; OnPropertyChanged(); } }

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

        private ObservableCollection<HistoryObject> historyObjectIns;
        public ObservableCollection<HistoryObject> HistoryObjectIns { get => historyObjectIns; set { historyObjectIns = value; OnPropertyChanged(); } }

        private ObservableCollection<HistoryObject> historyObjectOuts;
        public ObservableCollection<HistoryObject> HistoryObjectOuts { get => historyObjectOuts; set { historyObjectOuts = value; OnPropertyChanged(); } }

        private readonly MainViewStore mainStore;
        private readonly ModuleStore moduleStore;
        private readonly ModuleStateManagement moduleStateManagement;
        private readonly IServiceProvider serviceProvider;
        private readonly MainStateManagement mainStateManagement;
        private readonly testModuleViewModel testModuleVM;
        private readonly HistoryDataStore historyDataStore;

        public BaseViewModel CurrentModuleViewModel => mainStore.CurrentViewModel;
        /*public ModuleParameterViewModel ModuleParameterViewModel { get => moduleParameterViewModel; set { moduleParameterViewModel = value; OnPropertyChanged(); } }*/
        /*private ModuleParameterStore moduleParameterStore = new ModuleParameterStore();*/
        /*public ModuleParameterStore ModuleParameterStore { get => moduleParameterStore; set { moduleParameterStore = value; OnPropertyChanged(); } }*/
        public ICommand OpenDialogCommand { get; set; }
        public ICommand UpdateModuleCommand { get; set; }
        public ICommand LoadHistoryCommand { get; set; }
        public ICommand RunEnvironmentCommand { get; set; }
        public ICommand PauseEnvironmentCommand { get; set; }
        public ICommand StopEnvironmentCommand { get; set; }
        public ICommand LoadPorts { get; set; }

        public ICommand testCommand { get; set; }
        public ICommand autoSaveCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }


        ~MainViewModel() { }
        public MainViewModel(MainViewStore mainStore, MainStateManagement mainStateManagement, ModuleStateManagement moduleStateManagement,
            ModuleStore moduleStore, IServiceProvider serviceProvider, testModuleViewModel testModuleVM, HistoryDataStore historyDataStore)
        {

            //DI
            this.mainStore = mainStore;
            this.moduleStore = moduleStore;
            this.moduleStateManagement = moduleStateManagement;
            this.serviceProvider = serviceProvider;
            this.mainStateManagement = mainStateManagement;
            this.testModuleVM = testModuleVM;
            this.historyDataStore = historyDataStore;
            //Variable
            moduleObjects = new ObservableCollection<ModuleObject>();
            Ports = new ObservableCollection<ButtonPort>();
            HistoryObjectIns = new ObservableCollection<HistoryObject>();
            HistoryObjectOuts = new ObservableCollection<HistoryObject>();

            //Event delegate
            this.moduleStateManagement.ModuleObjectCreated += OnModuleObjectCreated;
            this.moduleStateManagement.ChangePositionAndPort += ExecuteAutoSavePosition;
            this.moduleStateManagement.DeleteModule += OnDeleteModule;
            this.mainStateManagement.IsRunningNow += OnIsRunningNow;
            this.mainStateManagement.IsIdleNow += OnIsIdleNow;
            this.mainStateManagement.IsPauseNow += OnIsPauseNow;
            this.mainStateManagement.UpdateHistoryOut += OnUpdateHistoryOut;
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
            StopEnvironmentCommand = new RelayCommand(() => ExecuteStopEnvironment());
            PauseEnvironmentCommand = new RelayCommand(() => ExecutePauseEnvironment());
            LoadPorts = new RelayCommand(() => ExecuteLoadPorts());
        }
        //Delegate handler
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
        private void OnUpdateHistoryOut(string portName)
        {
            var moduleHistory = historyDataStore.ModuleHistories.FirstOrDefault(x => x.moduleObject.port == portName);
            if (moduleHistory != null)
            {
                HistoryObjectOuts = new ObservableCollection<HistoryObject>(moduleHistory.historyObjectOuts);
            }
        }
        private void OnModuleObjectCreated(string portName)
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
            IsEnableHistory = false;
        }
        private void OnDeleteModule(string portName)
        {
            var tmp_ports = Ports;
            var p = tmp_ports.FirstOrDefault(x => x.PortName == portName);
            if (p != null)
            {
                p.Color = "Wheat";
            }
            Ports = tmp_ports;
        }
        //Command handler
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
        private void ExecuteClickPort(string portName)
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
                    var moduleHistory = historyDataStore.ModuleHistories.FirstOrDefault(x => x.moduleObject.port == portName);
                    if (moduleHistory != null)
                    {
                        HistoryObjectIns = new ObservableCollection<HistoryObject>(moduleHistory.historyObjectIns);
                        HistoryObjectOuts = new ObservableCollection<HistoryObject>(moduleHistory.historyObjectOuts);
                    }
                }
            }
        }
        private void OpenDialog(string portName)
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
                });
            }
        }
        private void ExecuteOpenUpdateModule(ModuleObject module)
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
                var moduleParameterVM = new ModuleParameterVM()
                {
                    id = module.id,
                    isEnableDelete = true,
                    listPort = tmp_Ports,
                    isUpdate = "true"
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
            IsEnableHistory = false;
        }

        private void CloseDialog(string port)
        {
            IsDialogOpen = false;
            ModuleObjects = new ObservableCollection<ModuleObject>(moduleStore.ModuleObjects);
            if (port != null)
            {
                serviceProvider.GetRequiredService<IEnvironmentService>().closePort(port);
            }
        }
        private void ExecuteLoadPorts()
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
        }

        private void DectectActionSelectedColumn()
        {
            //todo
            if (SelectedItemHistory != null)
            {
                SourceHistory = SelectedItemHistory.Source;
                DataHistory = SelectedItemHistory.Data;
                DelayTimeHistory = SelectedItemHistory.DelayTime == null ? "Na/N" : SelectedItemHistory.DelayTime;
            }
        }

        //Dispose
        public override void Dispose()
        {
            moduleStateManagement.ModuleObjectCreated -= OnModuleObjectCreated;
            base.Dispose();
        }
        //Invoke to Environment
        private void ExecuteRunEnvironment()
        {
            serviceProvider.GetRequiredService<IEnvironmentService>().passModuleObjects(new List<ModuleObject>(ModuleObjects));
            serviceProvider.GetRequiredService<IEnvironmentService>().Run();
        }
        private void ExecutePauseEnvironment()
        {
            serviceProvider.GetRequiredService<IEnvironmentService>().Pause();
        }
        private void ExecuteStopEnvironment()
        {
            serviceProvider.GetRequiredService<IEnvironmentService>().Stop();
        }
        //Received Data from Environment
        public void showQueueReceivedFromHardware(PacketTransferToView transferedPacket, string portClicked)
        {
            var moduleHistory = historyDataStore.ModuleHistories.FirstOrDefault(x => x.moduleObject.port == transferedPacket.portName);
            if (moduleHistory != null)
            {
                var newHistoryObject = new HistoryObject();
                if (moduleHistory.moduleObject.type == ModuleObject.LORA)
                {
                    var length = moduleHistory.historyObjectOuts.Count;
                    var loraParams = (LoraParameterObject)moduleHistory.moduleObject.parameters;

                    newHistoryObject = new HistoryObject()
                    {
                        Id = length + 1,
                        Source = "Address: " + loraParams.Address + "--- Channel: " + loraParams.Channel
                    };
                }
                else if (moduleHistory.moduleObject.type == ModuleObject.ZIGBEE)
                {

                }
                newHistoryObject.Data = transferedPacket.packet.data;
                moduleHistory.historyObjectOuts.Enqueue(newHistoryObject);
                mainStateManagement.updateHistoryOut(portClicked);
            }
        }

        public void deviceChangeMode(int mode, string port)
        {
            throw new NotImplementedException();
        }

        public void sendMessageIsRunning()
        {
            mainStateManagement.isRunningNow();
        }

        public void sendMessageIsIdle()
        {
            mainStateManagement.isIdleNow();
        }

        public void sendMessageIsStop()
        {
            mainStateManagement.isPauseNow();
        }
    }
}
