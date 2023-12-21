using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Simulator1.State_Management;
using Simulator1.Store;
using Environment.Model.History;
using Newtonsoft.Json;

namespace Simulator1.ViewModel
{
    public class ChartViewModel : BaseViewModel
    {
        private readonly MainStateManagement mainStateManagement;
        private readonly HistoryDataStore historyDataStore;

        private SeriesCollection seriesCollection = new SeriesCollection();
        public SeriesCollection SeriesCollection { get => seriesCollection; set { seriesCollection = value; OnPropertyChanged(); } }

        private List<string> listLabel = new List<string>();

        private string[] labels = new string[] { };
        public string[] Labels { get => labels; set { labels = value; OnPropertyChanged(); } }
        public Func<double, string> Formatter { get; set; }
        public ChartViewModel(MainStateManagement mainStateManagement, HistoryDataStore historyDataStore)
        {
            this.mainStateManagement = mainStateManagement;
            this.historyDataStore = historyDataStore;

            this.mainStateManagement.OpenChart += OnCreateChart;

            Formatter = value => value + " Packets";
        }
        private void OnCreateChart()
        {
            var receivedPacket = new ChartValues<double>();
            var lossPacket = new ChartValues<double>();
            foreach (var historyObject in historyDataStore.ModuleHistories)
            {
                receivedPacket.Add(historyObject.historyObjectIns.Count);
                lossPacket.Add(historyObject.historyObjectErrors.Count);
                listLabel.Add(getDeviceInfo(historyObject));
            }
            SeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Title = "Received Packets",
                    Values = receivedPacket,
                    StackMode = StackMode.Values, // this is not necessary, values is the default stack mode
                    DataLabels = true
                },
                new StackedColumnSeries
                {
                    Title="Loss Packets",
                    Values = lossPacket,
                    StackMode = StackMode.Values,
                    DataLabels = true
                }
            };
            Labels = listLabel.ToArray();
        }
        private string getDeviceInfo(ModuleHistory moduleHistory)
        {
            string json = JsonConvert.SerializeObject(moduleHistory.moduleObject.parameters);
            Dictionary<string, string> listParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var result = listParams["Address"] + " --- " + listParams["Channel"] + " --- " + moduleHistory.moduleObject.port;
            return result;
        }
    }
}
