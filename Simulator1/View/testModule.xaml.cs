using Simulator1.Model;
using Simulator1.Store;
using Simulator1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
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
    /// Interaction logic for testModule.xaml
    /// </summary>
    public partial class testModule : UserControl
    {
        public testModuleViewModel test { get; set; }
        protected bool isDragging;
        private Point clickPosition;
        private Point currentPosition;
        private Double prevX, prevY;
        private readonly ModuleStore moduleStore;




        /*public int Id
        {
            get { return (int)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Id.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(testModule), new PropertyMetadata(null));
*/

        public ICommand DropModuleCommand
        {
            get { return (ICommand)GetValue(DropModuleCommandProperty); }
            set { SetValue(DropModuleCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropModuleCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropModuleCommandProperty =
            DependencyProperty.Register("DropModuleCommand", typeof(ICommand), typeof(testModule), new PropertyMetadata(null));

        public testModule()
        {
            InitializeComponent();
            this.DataContext = test = new testModuleViewModel();
            this.MouseLeftButtonDown += new MouseButtonEventHandler(Control_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(Control_MouseLeftButtonUp);
            this.MouseMove += new MouseEventHandler(Control_MouseMove);
        }
        private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            var draggableControl = sender as UserControl;
            clickPosition = e.GetPosition(FindAncestor(this) as UIElement);
            draggableControl.CaptureMouse();
        }

        private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            var draggable = sender as UserControl;
            var transform = (draggable.RenderTransform as TranslateTransform);
            if (transform != null)
            {
                prevX = transform.X;
                prevY = transform.Y;
            }
            draggable.ReleaseMouseCapture();
            var position = currentPosition;
            DropModuleCommand?.Execute(this);
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            var draggableControl = sender as UserControl;

            if (isDragging && draggableControl != null)
            {
                Point current_position = e.GetPosition(FindAncestor(this) as UIElement);

                var transform = draggableControl.RenderTransform as TranslateTransform;
                if (transform == null)
                {
                    transform = new TranslateTransform();
                    draggableControl.RenderTransform = transform;
                }

                transform.X = (currentPosition.X - clickPosition.X);
                transform.Y = (currentPosition.Y - clickPosition.Y);
                if (prevX > 0)
                {
                    transform.X += prevX;
                    transform.Y += prevY;
                }
                currentPosition = current_position;
            }
        }
        private Canvas FindAncestor(DependencyObject obj)
        {
            DependencyObject tmp = VisualTreeHelper.GetParent(obj);
            while (tmp != null && !(tmp is Canvas))
            {
                tmp = VisualTreeHelper.GetParent(tmp);
            }
            return tmp as Canvas;
        }
    }
}
