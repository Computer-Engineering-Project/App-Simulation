using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Service.Interface
{
    public interface ICommunicationService
    {
        public Task listenToHardwareAsync(object sender);
    }
}
