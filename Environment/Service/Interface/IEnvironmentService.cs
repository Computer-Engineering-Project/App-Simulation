using Environment.Model.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Service.Interface
{
    public interface IEnvironmentService
    {
        public List<string> getPorts();
        public void startPort(string portName);
        public void ActiveHardware(string portName);
        public string getIdTypeFromHardware(string portName);
        public void closePort(string portName);
        public void passModuleObjects(List<ModuleObject> moduleObjects);
        public void changeModeDevice(string portName, string mode);
        public void Run();
    }
}
