using Simulator1.Model;
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
        public ICommand SavePositionCommand { get; set; }
        public testModuleViewModel()
        {
            SavePositionCommand = new ParameterRelayCommand<ModuleObject>((o)=> { return true; },(o) =>
            {
                ExecuteTest(o);
            });
        }
        private void ExecuteTest(ModuleObject o)
        {
            var p = o;
        }
    }
}
