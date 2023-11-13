using Simulator1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator1.Store
{
    public class ModuleParameterViewStore : NavigationStore
    {
        private BaseViewModel currentModuleViewModel;
        public override BaseViewModel CurrentViewModel
        {
            get => currentModuleViewModel;
            set
            {
                currentModuleViewModel?.Dispose();
                currentModuleViewModel = value;
                OnCurrentViewModelChanged();
            }
        }
        public event Action CurrentModuleViewModelChanged;
        public override void OnCurrentViewModelChanged()
        {
            CurrentModuleViewModelChanged?.Invoke();
        }
    }
}
