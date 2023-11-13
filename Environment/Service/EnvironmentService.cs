using Environment.Base;
using Environment.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Environment.Service
{
    public class EnvironmentService : IEnvironmentService
    {
        private readonly BaseEnvironment environment;
        public EnvironmentService()
        {
            environment = new BaseEnvironment(); 
        }
        public List<string> getPorts()
        {
            return environment.Ports;
        }
    }
}
