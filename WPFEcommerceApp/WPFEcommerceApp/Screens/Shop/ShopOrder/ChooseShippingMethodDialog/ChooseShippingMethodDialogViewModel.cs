using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Security.RightsManagement;
using System.Security;

namespace WPFEcommerceApp
{
    public class ChooseShippingMethodDialogViewModel:BaseViewModel
    {
        private ShopOrderBlockViewModel shopOrderBlockView;
        public ShopOrderBlockViewModel ShopOrderBlockViewModel
        {
            get => shopOrderBlockView;
            set
            {
                shopOrderBlockView = value;
                OnPropertyChanged(nameof(shopOrderBlockView));
            }
        }
        public ICommand ChooseCommand { get; set; }
        public ChooseShippingMethodDialogViewModel(ShopOrderBlockViewModel shopOrderBlockView)
        {
            ShopOrderBlockViewModel = shopOrderBlockView;
            ChooseCommand = new RelayCommand<object>(((p) => p != null), ((p) =>
            {
                string temp = p as string;
                if (temp == "Shipper")
                {
                    DialogHost.CloseDialogCommand.Execute(new Tuple<int, ShopOrderBlockViewModel>(1, ShopOrderBlockViewModel), null);
                }
                else if (temp == "Post")
                {
                    DialogHost.CloseDialogCommand.Execute(new Tuple<int, ShopOrderBlockViewModel>(2, ShopOrderBlockViewModel), null);
                }
            }));
        }
    }
}
