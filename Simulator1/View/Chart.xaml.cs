using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Simulator1.View
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class Chart : UserControl
    {
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        public Chart()
        {
            InitializeComponent();
            SeriesCollection = new SeriesCollection
            {
                new StackedColumnSeries
                {
                    Values = new ChartValues<double> {4, 5, 6, 8},
                    StackMode = StackMode.Values, // this is not necessary, values is the default stack mode
                    DataLabels = true
                },
                new StackedColumnSeries
                {
                    Values = new ChartValues<double> {2, 5, 6, 7},
                    StackMode = StackMode.Values,
                    DataLabels = true
                }
            };

            //adding series updates and animates the chart
            SeriesCollection.Add(new StackedColumnSeries
            {
                Values = new ChartValues<double> { 6, 2, 7 },
                StackMode = StackMode.Values
            });

            //adding values also updates and animates
            SeriesCollection[2].Values.Add(4d);

            Labels = new[] { "Chrome", "Mozilla", "Opera", "IE" };
            Formatter = value => value + " Mill";

            DataContext = this;
        }


    }

}
