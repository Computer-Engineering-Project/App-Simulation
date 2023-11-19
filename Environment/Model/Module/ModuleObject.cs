using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Model.Module
{
    public class ModuleObject
    {
        public const string LORA = "lora";
        public const string WIFI = "wifi";
        public string id { get; set; }
        public string port { get; set; }
        public double  x { get; set; }
        public double y { get; set; }
        public string type { get; set; }
        public object parameters { get; set; }
    }
}
