﻿using DataAccessLayer;
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
                if(!value)
                {
                    MaxSale = 0;
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
        private int targetCustomer;
        public int TargetCustomer
        {
            get => targetCustomer;
            set
            {
                targetCustomer = value;
                OnPropertyChanged();
            }
        }
        private string code;
        public string Code
        {
            get => code;
            set
            {
                code = value;
                OnPropertyChanged();
            }    
        }
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        private DateTime? dateBegin;
        public DateTime? DateBegin
        {
            get => dateBegin;
            set
            {
                dateBegin = value;
                OnPropertyChanged();
            }
        }
        private DateTime? dateEnd;
        public DateTime? DateEnd
        {
            get => dateEnd;
            set
            {
                dateEnd = value;
                OnPropertyChanged();
            }
        }
        private string description;
        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged();
            }
        }
        private int amount;
        public int Amount
        {
            get => amount;
            set
            {
                amount = value;
                OnPropertyChanged();
            }
        }
        private int sale;
        public int Sale
        {
            get => sale;
            set
            {
                sale = value;
                OnPropertyChanged();
            }
        }
        private double minCost;
        public double MinCost
        {
            get => minCost;
            set
            {
                minCost = value;
                OnPropertyChanged();
            }
        }
        private double maxSale;
        public double MaxSale
        {
            get => maxSale;
            set
            {
                maxSale = value;
                OnPropertyChanged();
            }
        }
        public AddShopPromoViewModel()
        {
            IsAdmin = isAdmin;
            IsMaxSale = false;
            SelectedProductPromos = new ObservableCollection<PromoProductBlockViewModel>();
            FilterProductPromos = SelectedProductPromos;
            SearchBy = "Id";
            AddNewProductCommand = new RelayCommandWithNoParameter(() =>
            {
                AddNewProductPromo addNewProductPromo = new AddNewProductPromo();
                addNewProductPromo.DataContext = new AddNewProductPromoViewModel(SelectedProductPromos);
                DialogHost.Show(addNewProductPromo, "Main", null, null, LoadList);
            });
            SaveCommand = new RelayCommand<object>((p) =>
            {
                return !String.IsNullOrEmpty(Code) &&
                        !String.IsNullOrEmpty(Name) &&
                        DateBegin != null &&
                        DateEnd != null &&
                        !String.IsNullOrEmpty(Description) &&
                        Amount > 0 &&
                        Sale > 0 &&
                        MinCost >= 0 &&
                        MaxSale >= 0 ;
            },async (p) =>
            {
                await SaveProduct();
            });
            DeleteProductCommand = new RelayCommand<object>((p) => p != null, (p) =>
            {
                PromoProductBlockViewModel promoProductBlockViewModel = p as PromoProductBlockViewModel;
                SelectedProductPromos.Remove(promoProductBlockViewModel);
                FilterProductPromos.Remove(promoProductBlockViewModel);
            });
            SearchCommand = new RelayCommandWithNoParameter(() =>
            {
                Search();
            });
        }
        private async Task SaveProduct()
        {
            GenericDataRepository<Models.Promo> promoRepository = new GenericDataRepository<Promo>();
            string id = await GenerateID.Gen(typeof(Models.Promo));
            await promoRepository.Add(new Models.Promo()
            {
                Id = id,
                IdShop = AccountStore.instance.CurrentAccount.Id,
                Code = this.Code,
                Description = this.Description,
                DateBegin = this.DateBegin,
                DateEnd = this.DateEnd,
                Amount = this.Amount,
                AmountUsed = 0,
                MaxSale = (IsMaxSale?this.MaxSale:double.MaxValue),
                MinCost = this.MinCost,
                Sale = this.Sale,
                CustomerType = this.TargetCustomer,
                Name = this.Name
            }) ;
            foreach (PromoProductBlockViewModel promoProductBlock in SelectedProductPromos)
            {
                await PromoDetailAPI.Add(id, promoProductBlock.SelectedProduct.Id);   
            }    
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
                foreach (PromoProductBlockViewModel product in SelectedProductPromos)
                {
                    product.DeleteCommand = DeleteProductCommand;
                }
                Search();
            }       
        }
        private void Search()
        {
            if(SearchBy == "Id")
            {
                FilterProductPromos = new ObservableCollection<PromoProductBlockViewModel>(SelectedProductPromos.Where(p => p.SelectedProduct.Id.Contains(SearchByValue??"")));
            }    
            else if(SearchBy == "Name")
            {
                FilterProductPromos = new ObservableCollection<PromoProductBlockViewModel>(SelectedProductPromos.Where(p => p.SelectedProduct.Name.Contains(SearchByValue??"")));
            }    
        }
    }
}
