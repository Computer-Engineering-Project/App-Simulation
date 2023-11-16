using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator1.State_Management
{
    public class MainStateManagement
    {
        public event Action LoadHistory;
        public void loadHistoryFromDB()
        {
            LoadHistory?.Invoke();
        }
    }
}
