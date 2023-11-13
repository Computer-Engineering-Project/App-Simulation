using Environment.Service;
using Environment.Service.Interface;
using Simulator1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator1.Store
{
    public class ModuleStore
    {
        public List<ModuleObject> ModuleObjects { get; set; }
        public List<string> Ports { get; set; }
        public ModuleStore(IEnvironmentService environmentService)
        {
            ModuleObjects = new List<ModuleObject>();
            Ports = new List<string>(environmentService.getPorts());
        }
    }
}
