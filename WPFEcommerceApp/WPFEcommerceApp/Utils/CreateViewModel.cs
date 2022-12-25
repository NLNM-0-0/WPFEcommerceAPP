using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public delegate TViewModel CreateViewModel<TViewModel>() where TViewModel : BaseViewModel;

    public delegate TViewModel CreateParamVM<TViewModel>(object parameter) where TViewModel : BaseViewModel;
}
