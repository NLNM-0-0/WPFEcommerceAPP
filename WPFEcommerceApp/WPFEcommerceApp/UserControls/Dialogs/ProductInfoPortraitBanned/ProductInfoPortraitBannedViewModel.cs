﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFEcommerceApp
{
    public class ProductInfoPortraitBannedViewModel : BaseViewModel
    {
        public ICommand OpenProductInfoLandscapeCommand { get; set; }
        private Models.Product selectedProduct;
        public Models.Product SelectedProduct
        {
            get
            {
                if (selectedProduct == null)
                {
                    selectedProduct = new Models.Product();
                }
                return selectedProduct;
            }
            set
            {
                selectedProduct = value;
                OnPropertyChanged();
            }
        }
        public ProductInfoPortraitBannedViewModel(Models.Product product)
        {
            SelectedProduct = product;

            OpenProductInfoLandscapeCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                ProductInfoLandscape productInfoLandscape = new ProductInfoLandscape();
                productInfoLandscape.DataContext = new ProductInfoLandscapeViewModel(SelectedProduct);
                DialogHost.Show(productInfoLandscape);
            });
        }
    }
}