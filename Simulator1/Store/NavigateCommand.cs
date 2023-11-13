using Simulator1.Service;
using Simulator1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator1.Store
{
    public class NavigateCommand : CommandBase
    {
        private readonly INavigateService navigateService;

        public NavigateCommand(INavigateService navigateService)
        {
            this.navigateService = navigateService;
        }
        public override void Execute(object parameter)
        {
            navigateService.Navigate();
        }
    }
}
