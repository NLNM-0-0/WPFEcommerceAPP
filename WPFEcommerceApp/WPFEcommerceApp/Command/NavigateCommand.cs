using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WPFEcommerceApp;

namespace WPFEcommerceApp {
    public class NavigateCommand : CommandBase{
        public NavigateCommand(INavigationService navigateService) {
            this.navigateService = navigateService;
        }

        private readonly INavigationService navigateService;


        public override void Execute(object parameter) {
            navigateService.Navigate();
        }
    }
}
