using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPFEcommerceApp
{
    public class ProductBlockByCategoryViewModel : BaseViewModel
    {
        public ICommand ChangeToFilterCommand { get; set; }
        public double MaxHeightListProducts
        {
            get
            {
                if (IsFull)
                {
                    return double.PositiveInfinity;
                }
                else
                {
                    return 900;
                }
            }
        }
        private string header;
        public string Header
        {
            get => header;
            set
            {
                header = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Models.Product> fullProducts;
        public ObservableCollection<Models.Product> FullProducts
        {
            get => fullProducts;
            set
            {
                fullProducts = value;
                OnPropertyChanged();
            }
        }
        private bool isHidden;
        public bool IsHidden
        {
            get => isHidden;
            set
            {
                isHidden = value;
            }
        }
        private bool isFull;
        public bool IsFull
        {
            get => isFull;
            set
            {
                isFull = value;
            }
        }
        private ObservableCollection<ProductBlockViewModel> productBlockViewModel;
        public ObservableCollection<ProductBlockViewModel> ProductBlockViewModels
        {
            get => productBlockViewModel;
            set
            {
                productBlockViewModel = value;
                OnPropertyChanged();
            }
        }
        private bool isShop;
        public bool IsShop
        {
            get => isShop;
            set
            {
                isShop = value;
                OnPropertyChanged();
            }
        }
        public ProductBlockByCategoryViewModel() { }
        public ProductBlockByCategoryViewModel(string header, ObservableCollection<Models.Product> products, bool isHidden, bool isFull, bool isShop)
        {
            Header = header;
            IsFull = isFull;
            IsHidden = isHidden;
            IsShop = isShop;
            if (products == null || products.Count == 0)
            {
                FullProducts = new ObservableCollection<Models.Product>();
            }
            else
            {
                FullProducts = products;
            }
            ProductBlockViewModels = new ObservableCollection<ProductBlockViewModel>();
            foreach(Models.Product product in products) 
            {
                ProductBlockViewModel productBlockViewModel = new ProductBlockViewModel(product);
                productBlockViewModel.IsShop = this.IsShop;
                ProductBlockViewModels.Add(productBlockViewModel);
            }
            ChangeToFilterCommand = new RelayCommandWithNoParameter(() =>
            {
                MessageBox.Show("Change to filter");
            });
        }
    }
}
