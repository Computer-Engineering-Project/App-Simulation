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
            this.mainStateManagement.UpdateHitoryIn += OnUpdateHistoryIn;
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
            try
            {
                var moduleHistory = historyDataStore.ModuleHistories.FirstOrDefault(x => x.moduleObject.port == portName);
                if (moduleHistory != null)
                {
                    HistoryObjectOuts = new ObservableCollection<HistoryObject>(moduleHistory.historyObjectOuts);
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
                    HistoryObjectIns = new ObservableCollection<HistoryObject>(moduleHistory.historyObjectIns);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "OnUpdateHistoryIn " + e);
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
                IsEnableHistory = false;
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
        //Command handler
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
                        var moduleHistory = historyDataStore.ModuleHistories.FirstOrDefault(x => x.moduleObject.port == portName);
                        if (moduleHistory != null)
                        {
                            HistoryObjectIns = new ObservableCollection<HistoryObject>(moduleHistory.historyObjectIns);
                            HistoryObjectOuts = new ObservableCollection<HistoryObject>(moduleHistory.historyObjectOuts);
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
                if (!IsEnableRun && !IsEnablePause)
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
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "ExecuteOpenUpdateModule " + e);
            }

        }
        private void ExecuteLoadHistory()
        {
            try
            {
                mainStateManagement.loadHistoryFromDB();
                ModuleObjects = new ObservableCollection<ModuleObject>(moduleStore.ModuleObjects);
                IsEnableHistory = false;
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
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "DectectActionSelectedColumn " + e);
            }
            //todo

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
            if(ModuleObjects.Count == 0)
            {
                return false;
            }
            foreach(var m in ModuleObjects)
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
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "showQueueReceivedFromHardware " + e);
            }
            
        }
        public void showQueueReceivedFromOtherDevice(PacketReceivedTransferToView transferedPacket, string portClicked)
        {
            try
            {
                var moduleHistory = historyDataStore.ModuleHistories.FirstOrDefault(x => x.moduleObject.port == transferedPacket.portName);
                if (moduleHistory != null)
                {
                    var newHistoryObject = new HistoryObject();
                    if (moduleHistory.moduleObject.type == ModuleObject.LORA)
                    {
                        var length = moduleHistory.historyObjectIns.Count;
                        var loraParams = (LoraParameterObject)moduleHistory.moduleObject.parameters;

                        newHistoryObject = new HistoryObject()
                        {
                            Id = length + 1,
                            Source = "Port: " + transferedPacket.packet.sourceModule.port
                        };
                    }
                    else if (moduleHistory.moduleObject.type == ModuleObject.ZIGBEE)
                    {

                    }
                    newHistoryObject.Data = transferedPacket.packet.packet.data;
                    moduleHistory.historyObjectIns.Enqueue(newHistoryObject);
                    mainStateManagement.updateHistoryIn(portClicked);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Main view model " + "showQueueReceivedFromOtherDevice " + e);
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
