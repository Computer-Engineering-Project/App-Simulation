﻿using Environment.Model.Module;
﻿using Environment.Model.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Service.Interface
{
    public interface IEnvironmentService
    {
        public void startPort(string portName);
        public void ActiveHardware(string portName);
        public PacketTransmit getIdTypeFromHardware(string portName);
        public bool configHardware(string portName, object parameters);
        public void closePort(string portName);
        public void passModuleObjects(List<ModuleObject> moduleObjects);
        public List<string> loadPorts();
        public void Run();
        public void Stop();
    }
}
