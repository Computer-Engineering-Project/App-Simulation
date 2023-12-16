using Simulator1.ViewModel;
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

namespace Simulator1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isLoaded = false;
        private double _scaleValue = 1.0;
        public MainWindow()
        {
            if (!isLoaded)
            {
                InitializeComponent();
                isLoaded = true;
            }
        }
        private void Main_Window_Closed(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Control_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool zoomIn = e.Delta > 0;

            // Set the scale value based on the direction of the zoom
            _scaleValue += zoomIn ? 0.03 : -0.03;

            // Set the maximum and minimum scale values
            _scaleValue = _scaleValue < 1.0 ? 1.0 : _scaleValue;
            _scaleValue = _scaleValue > 10.0 ? 10.0 : _scaleValue;
            var centerX = (double)e.GetPosition(mycanvas).X;
            var centerY = (double)e.GetPosition(mycanvas).Y;

            // Apply the scale transformation to the ItemsControl
            ScaleTransform scaleTransform = new ScaleTransform(_scaleValue, _scaleValue, centerX, centerY);
            mycanvas.RenderTransform = scaleTransform;
        }
    }
}
