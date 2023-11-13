using Simulator1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator1.Store
{
    public class MainStore : NavigationStore
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
        public event Action MainCurrentModuleViewModelChanged;
        public override void OnCurrentViewModelChanged()
        {
            MainCurrentModuleViewModelChanged?.Invoke();
        }
    }
}
