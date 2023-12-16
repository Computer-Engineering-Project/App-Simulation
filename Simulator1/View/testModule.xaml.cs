
using Simulator1.State_Management;
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
        /*        public testModuleViewModel test { get; set; }*/
        protected bool isDragging;
        private FrameworkElement dragElement;
        private Point clickPosition;
        private Double baseX, baseY = 0;
        private Double transX, transY = 0;
        private Point intialActualElementOffset;
        private Point intialTransformElementOffset;
        private Point currentPosition;
        private Double prevX, prevY;




        public string Id
        {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Id.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(string), typeof(testModule), new PropertyMetadata(null));

        public string ModeModule
        {
            get { return (string)GetValue(ModeModuleProperty); }
            set { SetValue(ModeModuleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ModeModule.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeModuleProperty =
            DependencyProperty.Register("ModeModule", typeof(string), typeof(testModule), new PropertyMetadata(null));

        public string PortModule
        {
            get { return (string)GetValue(PortModuleProperty); }
            set { SetValue(PortModuleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Id.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PortModuleProperty =
            DependencyProperty.Register("PortModule", typeof(string), typeof(testModule), new PropertyMetadata(null));

        public double CoveringArea
        {
            get { return (double)GetValue(CoveringAreaProperty); }
            set { SetValue(CoveringAreaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CoveringArea.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CoveringAreaProperty =
            DependencyProperty.Register("CoveringArea", typeof(double), typeof(testModule), new PropertyMetadata(null));



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
            this.MouseLeftButtonDown += new MouseButtonEventHandler(Control_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(Control_MouseLeftButtonUp);
            this.MouseMove += new MouseEventHandler(Control_MouseMove);
        }
        private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            var draggableControl = sender as UserControl;
            if (draggableControl != null)
            {
                dragElement = (FrameworkElement)sender;
            }

            intialActualElementOffset = e.GetPosition(dragElement.FindName("device") as UIElement);
            intialTransformElementOffset = e.GetPosition(dragElement as UIElement);
            if (intialActualElementOffset.X > 0 && intialActualElementOffset.X <= 40 && intialActualElementOffset.Y > 0 && intialActualElementOffset.Y <= 40)
            {
                draggableControl.CaptureMouse();
                isDragging = true;
                clickPosition = e.GetPosition(FindAncestor(this) as UIElement);
            }

        }

        private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                var draggable = sender as UserControl;
                var transform = (draggable.RenderTransform as TranslateTransform);
                var modulePosition = e.GetPosition(dragElement);
                if (transform != null)
                {
                    prevX = transform.X;
                    prevY = transform.Y;
                }
                draggable.ReleaseMouseCapture();
                if (currentPosition.X > 0 && currentPosition.Y > 0)
                {
                    transX = currentPosition.X - intialTransformElementOffset.X;
                    transY = currentPosition.Y - intialTransformElementOffset.Y;
                    baseX = transX + CoveringArea / 2 - 20;
                    baseY = transY + CoveringArea / 2 - 20;
                    var id = ((testModule)draggable).Id;
                    DropModuleCommand?.Execute(new
                    {
                        x = baseX * 10,
                        y = baseY * 10,
                        transformX = transX,
                        transformY = transY,
                        id = id,
                    });
                }

            }

        }
        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (intialActualElementOffset.X > 0 && intialActualElementOffset.X <= 40 && intialActualElementOffset.Y > 0 && intialActualElementOffset.Y <= 40)
            {
                var draggableControl = sender as UserControl;

                if (isDragging && draggableControl != null && e.LeftButton == MouseButtonState.Pressed)
                {
                    Point current_position = e.GetPosition(FindAncestor(this) as UIElement);
                    intialActualElementOffset = e.GetPosition(dragElement.FindName("device") as UIElement);
                    intialTransformElementOffset = e.GetPosition(dragElement as UIElement);

                    var transform = draggableControl.RenderTransform as TranslateTransform;
                    if (transform == null)
                    {
                        transform = new TranslateTransform();
                        draggableControl.RenderTransform = transform;
                    }

                    transform.X = (current_position.X - clickPosition.X);
                    transform.Y = (current_position.Y - clickPosition.Y);
                    if (prevX > 0)
                    {
                        transform.X += prevX;
                        transform.Y += prevY;
                    }
                    currentPosition = current_position;
                }
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
