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
    public class PromoInformationViewModel : BaseViewModel
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
                if (SelectedPromo != null && isMaxSale && (SelectedPromo.MaxSale == null || SelectedPromo.MaxSale == double.MaxValue))
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
        private Models.Promo initialPromo;
        public PromoInformationViewModel(Models.Promo promo, bool isAdmin = false)
        {
            initialPromo = promo;
            AddNewProductCommand = new RelayCommandWithNoParameter(() =>
            {
                AddNewProductPromo addNewProductPromo = new AddNewProductPromo();
                addNewProductPromo.DataContext = new AddNewProductPromoViewModel(SelectedProductPromos, SelectedPromo.MUser);
                DialogHost.Show(addNewProductPromo, "Main", null, null, LoadList);
            });
            SaveCommand = new RelayCommandWithNoParameter(() =>
            {
                Task.Run(async () => await SaveProduct());
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
            #region Thu
            AcceptCommand = new RelayCommandWithNoParameter(() => { });
            DeleteCommand = new RelayCommandWithNoParameter(() => { });
            #endregion
            IsAdmin = isAdmin;
            Task.Run(async () => await Load()).Wait();
            if (SelectedPromo.MaxSale == null || SelectedPromo.MaxSale == double.MaxValue)
            {
                IsMaxSale = false;
            }
            else
            {
                IsMaxSale = true;
            }
            SelectedProductPromos = new ObservableCollection<PromoProductBlockViewModel>();
            foreach (Models.Product product in promo.Products)
            {
                PromoProductBlockViewModel promoProductBlockViewModel = new PromoProductBlockViewModel(product);
                promoProductBlockViewModel.DeleteCommand = DeleteProductCommand;
                SelectedProductPromos.Add(promoProductBlockViewModel);
            }
            FilterProductPromos = SelectedProductPromos;
            SearchBy = "Id";
        }
        private async Task SaveProduct()
        {
            GenericDataRepository<Models.Promo> promoRepository = new GenericDataRepository<Promo>();
            Models.Promo promo = await promoRepository.GetSingleAsync(p => p.Id == SelectedPromo.Id);
            promo.Code = SelectedPromo.Code;
            promo.Description = SelectedPromo.Description;
            promo.DateBegin = SelectedPromo.DateBegin;
            promo.DateEnd = SelectedPromo.DateEnd;
            promo.Amount = SelectedPromo.Amount;
            promo.AmountUsed = 0;
            promo.MaxSale = (IsMaxSale ? SelectedPromo.MaxSale : double.MaxValue);
            promo.MinCost = SelectedPromo.MinCost;
            promo.Sale = SelectedPromo.Sale;
            promo.CustomerType = SelectedPromo.CustomerType;
            promo.Name = SelectedPromo.Name;
            await promoRepository.Update(promo);
            foreach(Models.Product p in SelectedPromo.Products)
            {
                await PromoDetailAPI.Delete(promo.Id, p.Id);
            }
            SelectedPromo.Products.Clear();
            foreach (PromoProductBlockViewModel promoProductBlock in SelectedProductPromos)
            {
                await PromoDetailAPI.Add(promo.Id, promoProductBlock.SelectedProduct.Id);
                SelectedPromo.Products.Add(promoProductBlock.SelectedProduct);
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
        private async Task Load()
        {
            GenericDataRepository<Models.Promo> promoRepository = new GenericDataRepository<Models.Promo>();
            SelectedPromo = await promoRepository.GetSingleAsync(p => p.Id == initialPromo.Id,
                                                                p => p.MUser,
                                                                p => p.Products);
        }
        private void Search()
        {
            if (SearchBy == "Id")
            {
                FilterProductPromos = new ObservableCollection<PromoProductBlockViewModel>(SelectedProductPromos.Where(p => p.SelectedProduct.Id.Contains(SearchByValue??"")));
            }
            else if (SearchBy == "Name")
            {
                FilterProductPromos = new ObservableCollection<PromoProductBlockViewModel>(SelectedProductPromos.Where(p => p.SelectedProduct.Name.Contains(SearchByValue??"")));
            }
        }
    }
}
