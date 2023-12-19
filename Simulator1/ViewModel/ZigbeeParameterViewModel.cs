using Environment.Base;
using Environment.Model.Module;
using Environment.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Simulator1.State_Management;
using Simulator1.Store;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Net;
using System.Windows;

namespace Simulator1.ViewModel
{
    public class ZigbeeParameterViewModel : BaseViewModel
    {
        private ObservableCollection<string> listUartRate;
        public ObservableCollection<string> ListUartRate { get => listUartRate; set { listUartRate = value; OnPropertyChanged(); } }

        private ObservableCollection<string> listFixedMode;
        public ObservableCollection<string> ListFixedMode { get => listFixedMode; set { listFixedMode = value; OnPropertyChanged(); } }

        private string id;
        public string Id { get => id; set { id = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private string channel;
        public string Channel { get => channel; set { channel = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private string airRate;
        public string AirRate { get => airRate; set { airRate = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private string powerTransmit;
        public string PowerTransmit { get => powerTransmit; set { powerTransmit = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private string uartRate;
        public string UartRate { get => uartRate; set { uartRate = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private string fixedMode;
        public string FixedMode { get => fixedMode; set { fixedMode = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }

        private string antennaGain;
        public string AntennaGain { get => antennaGain; set { antennaGain = value; OnPropertyChanged(); statusStateManagement.statusChanged(); } }




        private readonly ModuleStateManagement moduleStateManagement;
        private readonly ModuleStore moduleStore;
        private readonly IServiceProvider serviceProvider;
        private readonly StatusStateManagement statusStateManagement;

        public ZigbeeParameterViewModel(ModuleStateManagement moduleStateManagement, ModuleStore moduleStore, IServiceProvider serviceProvider,
            StatusStateManagement statusStateManagement)
        {
            this.moduleStateManagement = moduleStateManagement;
            this.moduleStore = moduleStore;
            this.serviceProvider = serviceProvider;
            this.statusStateManagement = statusStateManagement;

            ListUartRate = new ObservableCollection<string>() { "2400", "4800", "9600", "19200", "38400", "57600", "115200" };

            ListFixedMode = new ObservableCollection<string>() { "Point-To-Point", "Broadcast" };

            //Event Delegate
            this.moduleStateManagement.ZigbeeParamsCreated += OnCreateZigbeeParameter;
            this.moduleStateManagement.UpdateZigbeeParamsOfModule += OnUpdateZigbeeParamsOfModule;
            this.moduleStateManagement.OpenUpdateZigbeeParams += OnOpenUpdateZigbeeParamter;
            this.moduleStateManagement.ReadZigbeeConfigParams += OnReadConfigZigbeeParameter;// load from database
            this.moduleStateManagement.ResetParameterModule += OnResetParameterModule;
        }
        private ZigbeeParameterObject createZigbeeParamsObject()
        {
            try
            {
                Channel = Channel == null ? "10" : Channel;
                UartRate = UartRate == null ? "9600" : UartRate;
                AirRate = AirRate == null ? "3300" : AirRate;
                PowerTransmit = PowerTransmit == null ? "20" : PowerTransmit;
                FixedMode = FixedMode == null ? TransmitMode.POINT_TO_POINT : FixedMode;
                AntennaGain = AntennaGain == "" || AntennaGain == null ? "0" : AntennaGain;

                return new ZigbeeParameterObject()
                {
                    Address = "MAC",
                    UartRate = UartRate,
                    Channel = Channel,
                    AirRate = AirRate,
                    Power = PowerTransmit,
                    TransmitMode = FixedMode,
                    AntennaGain = AntennaGain
                };
            }
            catch (Exception e)
            {

                MessageBox.Show("Zigbee paramter view model " + "createZigbeeParamsObject " + e);
                return null;
            }
        }
        private void OnCreateZigbeeParameter(ModuleObject moduleObject)
        {
            try
            {
                var zigbeeParams = createZigbeeParamsObject();
                moduleObject.parameters = zigbeeParams;
                moduleObject.type = ModuleObjectType.ZIGBEE;
                moduleObject.coveringAreaRange = CaculateService.computeRange(AntennaGain, PowerTransmit, 250);
                moduleObject.coveringAreaDiameter = moduleObject.coveringAreaRange / 5;

                var result = serviceProvider.GetRequiredService<IEnvironmentService>().configHardware(moduleObject.port, new
                {
                    module = moduleObject.type,
                    id = moduleObject.id,
                    baudrate = zigbeeParams.UartRate
                });
                if (result)
                {
                    moduleStateManagement.configHardwareSuccess(moduleObject);
                    MessageBox.Show("config object success!");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Zigbee paramter view model " + "OnCreateZigbeeParameter " + e);
            }
        }
        private void OnUpdateZigbeeParamsOfModule(ModuleObject moduleObject)
        {
            try
            {
                if (moduleObject.type == ModuleObjectType.ZIGBEE)
                {
                    var zigbeeParams = createZigbeeParamsObject();
                    moduleObject.parameters = zigbeeParams;
                    moduleObject.coveringAreaRange = CaculateService.computeRange(AntennaGain, PowerTransmit, 250);
                    moduleObject.coveringAreaDiameter = moduleObject.coveringAreaRange / 5;

                    var result = serviceProvider.GetRequiredService<IEnvironmentService>().configHardware(moduleObject.port, new
                    {
                        module = moduleObject.type,
                        id = moduleObject.id,
                        baudrate = zigbeeParams.UartRate
                    });
                    if (result)
                    {
                        moduleStateManagement.configHardwareSuccess(moduleObject);
                        MessageBox.Show("config object success!");
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Lora paramter view model " + "OnUpdateParamsOfModule " + e);
            }
        }
        private void OnReadConfigZigbeeParameter(Dictionary<string, string> listParams)
        {
            try
            {
                Channel = listParams["Channel"];
                AirRate = listParams["AirRate"];
                PowerTransmit = listParams["Power"];
                FixedMode = listParams["TransmitMode"];
                UartRate = listParams["UartRate"];
                AntennaGain = listParams["AntennaGain"];
            }
            catch (Exception e)
            {
                MessageBox.Show("Zigbee paramter view model " + "OnReadConfigZigbeeParameter " + e);
            }

        }
        private void OnResetParameterModule()
        {

            Channel = null;
            AirRate = null;
            PowerTransmit = null;
            FixedMode = null;
            UartRate = null;
            AntennaGain = null;
        }
        private void OnOpenUpdateZigbeeParamter(ZigbeeParameterObject zigbeeParams)
        {
            try
            {
                Channel = zigbeeParams.Channel;
                AirRate = zigbeeParams.AirRate;
                PowerTransmit = zigbeeParams.Power;
                FixedMode = zigbeeParams.TransmitMode;
                UartRate = zigbeeParams.UartRate;
                AntennaGain = zigbeeParams.AntennaGain;
            }
            catch (Exception e)
            {
                MessageBox.Show("Zigbee paramter view model " + "OnOpenUpdateZigbeeParamter " + e);
            }
        }
        public override void Dispose()
        {
            this.moduleStateManagement.ZigbeeParamsCreated -= OnCreateZigbeeParameter;
            this.moduleStateManagement.UpdateZigbeeParamsOfModule -= OnUpdateZigbeeParamsOfModule;
            this.moduleStateManagement.OpenUpdateZigbeeParams -= OnOpenUpdateZigbeeParamter;
            this.moduleStateManagement.ReadZigbeeConfigParams -= OnReadConfigZigbeeParameter;// load from database
            this.moduleStateManagement.ResetParameterModule -= OnResetParameterModule;
            base.Dispose();
        }
    }
}
