using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Model.ButtonPort
{
    public class ButtonPort : BaseModel
    {
        private string color;
        public string Color { get => color; set { color = value; OnPropertyChanged(); } }
        private string portName { get; set; }
        public string PortName { get => portName; set { portName = value; OnPropertyChanged(); } }
    }
}
