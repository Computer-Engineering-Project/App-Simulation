using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator1.State_Management
{
    public class StatusStateManagement
    {
        public event Action StatusChanged;
        public void statusChanged()
        {
            StatusChanged?.Invoke();
        }
    }
}
