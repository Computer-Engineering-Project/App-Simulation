using Simulator1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator1.Store
{
    public class MainViewStore : NavigationStore
    {
        private BaseViewModel currentViewModel;
        public override BaseViewModel CurrentViewModel
        {
            get => currentViewModel;
            set
            {
                currentViewModel?.Dispose();
                currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }

        private BaseViewModel currentModuleObjectViewModel;
        public BaseViewModel CurrentModuleObjectViewModel
        {
            get => currentModuleObjectViewModel;
            set
            {
                currentModuleObjectViewModel?.Dispose();
                currentModuleObjectViewModel = value;

            }
        }

        public event Action MainCurrentModuleViewModelChanged;
        public override void OnCurrentViewModelChanged()
        {
            MainCurrentModuleViewModelChanged?.Invoke();
        }
    }
}
