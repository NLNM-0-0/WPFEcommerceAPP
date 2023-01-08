using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class AddShopPromoViewModel: BaseViewModel
    {
        public ICommand AddNewProductCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteProductCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        #region Thu
        public ICommand AcceptCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        #endregion

        private string searchByValue;
        public string SearchByValue
        {
            get => searchByValue;
            set
            {
                searchByValue = value;
                OnPropertyChanged();    
            }
        }
        private bool isMaxSale;
        public bool IsMaxSale
        {
            get
            {
                return isMaxSale;
            }
            set
            {
                isMaxSale = value;
                if(SelectedPromo != null && isMaxSale && (SelectedPromo.MaxSale == null || SelectedPromo.MaxSale == double.MaxValue))
                {
                    SelectedPromo.MaxSale = 0;
                    OnPropertyChanged(nameof(SelectedPromo));
                }
                OnPropertyChanged();
            }
            
        }
        private ObservableCollection<PromoProductBlockViewModel> selectedProductPromos;
        public ObservableCollection<PromoProductBlockViewModel> SelectedProductPromos
        {
            get => selectedProductPromos;
            set
            {
                selectedProductPromos = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<PromoProductBlockViewModel> filterProductPromos;
        public ObservableCollection<PromoProductBlockViewModel> FilterProductPromos
        {
            get => filterProductPromos;
            set
            {
                filterProductPromos = value;
                OnPropertyChanged();
            }
        }
        private string searchBy;
        public string SearchBy
        {
            get => searchBy;
            set
            {
                searchBy = value;
                OnPropertyChanged();
            }
        }
        private bool isAdmin;
        public bool IsAdmin
        {
            get { return isAdmin; }

            set
            {
                isAdmin = value;
                OnPropertyChanged();
            }
        }
        private Models.Promo selectedPromo;
        public Models.Promo SelectedPromo
        {
            get { return selectedPromo; }
            set
            {
                selectedPromo = value;
                OnPropertyChanged();
            }
        }
        public AddShopPromoViewModel(Models.Promo promo, bool isAdmin = false)
        {
            IsAdmin = isAdmin;
            SelectedPromo = promo;
            if (SelectedPromo.MaxSale == null || SelectedPromo.MaxSale == double.MaxValue)
            {
                IsMaxSale = false;
            }
            else
            {
                IsMaxSale = true;
            }    
            SelectedProductPromos = new ObservableCollection<PromoProductBlockViewModel>();
            foreach(Models.Product product in promo.Products) 
            {
                PromoProductBlockViewModel promoProductBlockViewModel = new PromoProductBlockViewModel(product);
                promoProductBlockViewModel.DeleteCommand = DeleteProductCommand;
                SelectedProductPromos.Add(promoProductBlockViewModel);
            }
            FilterProductPromos = SelectedProductPromos;
            SearchBy = "Id";
            AddNewProductCommand = new RelayCommandWithNoParameter(() =>
            {
                AddNewProductPromo addNewProductPromo = new AddNewProductPromo();
                addNewProductPromo.DataContext = new AddNewProductPromoViewModel(SelectedPromo);
                DialogHost.Show(addNewProductPromo, "Main", null, null, LoadList);
            });
            SaveCommand = new RelayCommandWithNoParameter(() =>
            {

            });
            DeleteProductCommand = new RelayCommand<object>((p) => p != null, (p) =>
            {
                PromoProductBlockViewModel promoProductBlockViewModel = p as PromoProductBlockViewModel;
                SelectedProductPromos.Remove(promoProductBlockViewModel);
                SelectedPromo.Products.Remove(promoProductBlockViewModel.SelectedProduct);
            });
            SearchCommand = new RelayCommandWithNoParameter(() =>
            {
                Search();
            });
            #region Thu
            AcceptCommand = new RelayCommandWithNoParameter(() => { });
            DeleteCommand = new RelayCommandWithNoParameter(() => { });
            #endregion
        }

        private void LoadList(object sender, DialogClosedEventArgs eventArgs)
        {
            if (eventArgs == null || eventArgs.Parameter == null || eventArgs.Parameter.GetType() != typeof(ObservableCollection<PromoProductBlockViewModel>))
            {
                return;
            } 
            else
            {
                ObservableCollection<PromoProductBlockViewModel> result = eventArgs.Parameter as ObservableCollection<PromoProductBlockViewModel>;
                SelectedProductPromos = result;
                SelectedPromo.Products.Clear();
                foreach (PromoProductBlockViewModel product in SelectedProductPromos)
                {
                    SelectedPromo.Products.Add(product.SelectedProduct);
                    product.DeleteCommand = DeleteProductCommand;
                }
                Search();
            }       
        }
        private void Search()
        {
            if(SearchBy == "Id")
            {
                FilterProductPromos = new ObservableCollection<PromoProductBlockViewModel>(FilterProductPromos.Where(p => p.SelectedProduct.Id.Contains(SearchByValue)));
            }    
            else if(SearchBy == "Name")
            {
                FilterProductPromos = new ObservableCollection<PromoProductBlockViewModel>(FilterProductPromos.Where(p => p.SelectedProduct.Name.Contains(SearchByValue)));
            }    
        }
    }
}
