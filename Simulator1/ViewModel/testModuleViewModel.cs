using Simulator1.Model;
using Simulator1.State_Management;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Simulator1.ViewModel
{
    public class testModuleViewModel : BaseViewModel
    {
        public ICommand autoSaveCommand { get; set; }
        public testModuleViewModel()
        {
            autoSaveCommand = new ParameterRelayCommand<object>((o)=> { return true; },(o) =>
            {
                ExecuteTest(o);
            });
        }
        private void ExecuteTest(object o)
        {
            var p = o;
        }
    }
}
