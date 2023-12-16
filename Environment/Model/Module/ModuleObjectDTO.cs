using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Model.Module
{
    public class ModuleObjectDTO
    {
        public string id { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double coveringAreaRange { get; set; }
        public string type { get; set; }
        public object parameters { get; set; }
    }
}
