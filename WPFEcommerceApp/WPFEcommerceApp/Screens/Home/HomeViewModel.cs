using DataAccessLayer;
using MaterialDesignThemes.Wpf.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPFEcommerceApp
{
    internal class HomeViewModel:BaseViewModel
    {
        public GenericDataRepository<Models.Product> ProductRepository { get; set; }
        private ObservableCollection<Models.Product> products;
        public ObservableCollection<Models.Product> Products
        {
            get { return products; }
            set { products = value; 
            OnPropertyChanged();}
            
        }
        private Models.Product selectedProduct;
        public Models.Product SelectedProduct
        {
            get => selectedProduct;
            set => selectedProduct = value;
        }
        private bool newProduct;
        public bool NewProdcut
        {
            get => newProduct;
            set
            {
                newProduct = value;
                if (value)
                {
                    
                }
            }
        }
        private bool sortPrice0To200k;
        public bool SortPrice0To200k
        {
            get => sortPrice0To200k;
            set
            {
                sortPrice0To200k = value;
                if (value)
                {
                    Task.Run(async()=>await LoadPrice0To200k());
                }
            }
        }
        private bool sortPrice200kTo500k;
        public bool SortPrice200kTo500k
        {
            get => sortPrice200kTo500k;
            set
            {
                sortPrice200kTo500k = value;
                if(value)
                {
                    Task.Run(async () => await LoadPrice200kTo500k());
                }
            }
        }
        private bool sortPrice500kTo1000k;
        public bool SortPrice500kTo1000k
        {
            get => sortPrice500kTo1000k;
            set
            {
                sortPrice500kTo1000k = value;
                if (value)
                {
                    Task.Run(async () => await LoadPrice500kTo1000k());
                }
            }
        }
        private bool sortPriceP1000k;
        public bool SortPriceP1000k
        {
            get => sortPriceP1000k;
            set
            {
                sortPriceP1000k = value;
                if (value)
                {
                    Task.Run(async () => await LoadPriceP1000k());
                }
            }
        }
        public HomeViewModel()
        {
            ProductRepository = new GenericDataRepository<Models.Product>();
            Load();
        }
        private async void Load()
        {
            Products = new ObservableCollection<Models.Product>( await ProductRepository.GetListAsync(p=>p!=null,
                                                                                                        p=>p.Category,
                                                                                                        p=>p.Brand,
                                                                                                        p=>p.ImageProducts));
        }
        private async Task LoadPrice0To200k()
        {
            Products = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p => (p.Price*(100 - p.Sale)/100 <= 200000 && p.Price * (100 - p.Sale) / 100 >= 0),
                                                                                                        p => p.Category,
                                                                                                        p => p.Brand,
                                                                                                        p => p.ImageProducts));
            OnPropertyChanged(nameof(Products));
        }
        private async Task LoadPrice200kTo500k()
        {
            Products = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p => (p.Price * (100 - p.Sale) / 100 <= 500000 && p.Price * (100 - p.Sale) / 100 >= 200000),
                                                                                                        p => p.Category,
                                                                                                        p => p.Brand,
                                                                                                        p => p.ImageProducts));
            OnPropertyChanged(nameof(Products));
        }
        private async Task LoadPrice500kTo1000k()
        {
            Products = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p => (p.Price * (100 - p.Sale) / 100 <= 1000000 && p.Price * (100 - p.Sale) / 100 >= 500000),
                                                                                                        p => p.Category,
                                                                                                        p => p.Brand,
                                                                                                        p => p.ImageProducts));
            OnPropertyChanged(nameof(Products));
        }
        private async Task LoadPriceP1000k()
        {
            Products = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p => p.Price * (100 - p.Sale) / 100 >= 1000000,
                                                                                                        p => p.Category,
                                                                                                        p => p.Brand,
                                                                                                        p => p.ImageProducts));
            OnPropertyChanged(nameof(Products));
        }
        
    }
}
