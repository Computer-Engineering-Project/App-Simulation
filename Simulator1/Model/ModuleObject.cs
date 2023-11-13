using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator1.Model
{
    public class ModuleObject
    {
        public string port { get; set; }
        public double  x { get; set; }
        public double y { get; set; }
        public string type { get; set; }
        public object parameters { get; set; }
    }
}
