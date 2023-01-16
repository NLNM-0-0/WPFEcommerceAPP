using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WPFEcommerceApp
{
    internal class ProductDetailViewModel : BaseViewModel
    {
        private MainItem selectedItem;
        public MainItem SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                OnPropertyChanged();
            }
        }
        public ProductDetailViewModel(Models.Product product)
        {
            Task.Run(() =>
            {
                if (product.BanLevel != 0)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        IsLoadingCheck.IsLoading = 2;
                    }));
                    SelectedItem = new MainItem("ProductDetailBanned", typeof(ProductDetailBanned), new ProductDetailBannedViewModel());
                }
                else
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        IsLoadingCheck.IsLoading = 3;
                    }));
                    SelectedItem = new MainItem("ProductDetailNormal", typeof(ProductDetailNormal), new ProductDetailNormalViewModel(product));
                }
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            });
        }
    }
}
