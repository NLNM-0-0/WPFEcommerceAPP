using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp {
    public delegate TViewModel CreateViewModel<TViewModel>() where TViewModel : BaseViewModel;

    public delegate TViewModel CreateViewModel<TParameter, TViewModel>(TParameter parameter) where TViewModel : BaseViewModel;
}
