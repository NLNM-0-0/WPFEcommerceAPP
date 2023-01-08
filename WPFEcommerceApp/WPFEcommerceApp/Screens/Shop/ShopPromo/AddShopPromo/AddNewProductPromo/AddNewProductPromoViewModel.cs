﻿using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFEcommerceApp
{
    public class AddNewProductPromoViewModel : BaseViewModel
    {
        public ICommand SearchCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand CompletedCommand { get; set; }
        public ICommand DeleteProductCommand { get; set; }
        public ICommand ClickProductCommand { get; set; }

        private string searchBy;
        public string SearchBy
        {
            get { return searchBy; }
            set
            {
                searchBy = value;
                OnPropertyChanged();
            }
        }
        private string searchByValue;
        public string SearchByValue
        {
            get { return searchByValue; }
            set
            {
                searchByValue = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Models.Category> categories;
        public ObservableCollection<Models.Category> Categories
        {
            get { return categories; }
            set
            {
                categories = value;
                OnPropertyChanged();
            }
        }
        private Models.Category selectedCategory;
        public Models.Category SelectedCategory
        {
            get { return selectedCategory; }
            set 
            {
                selectedCategory = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Models.Brand> brands;
        public ObservableCollection<Models.Brand> Brands
        {
            get { return brands; }
            set
            {
                brands = value;
                OnPropertyChanged();
            }
        }
        private Models.Brand selectedBrand;
        public Models.Brand SelectedBrand
        {
            get { return selectedBrand; }
            set
            {
                selectedBrand = value;
                OnPropertyChanged();
            }
        }
        private bool isShowSelectedProductOnly;
        public bool IsShowSelectedProductOnly
        {
            get { return isShowSelectedProductOnly; }
            set
            {
                isShowSelectedProductOnly = value;
                if(!value)
                {
                    if(NumberOfCheck != AllPromoProductBlocks.Count)
                    {
                        isCheckedAll = false;
                        OnPropertyChanged(nameof(IsCheckedAll));
                    }    
                }    
                else
                {
                    isCheckedAll = true;
                    OnPropertyChanged(nameof(IsCheckedAll));
                }    
                Search();
                OnPropertyChanged();
            }
        }
        private bool isCheckedAll;
        public bool IsCheckedAll
        {
            get { return isCheckedAll; }
            set
            {
                isCheckedAll = value;
                OnPropertyChanged();
                if (isCheckedAll)
                {
                    foreach (PromoProductBlockViewModel promoProductBlock in FilterPromoProductBlocks)
                    {
                        if (!promoProductBlock.IsChecked)
                        {
                            TotalProducts++;
                            SelectedPromoProductBlocks.Insert(0, promoProductBlock);
                        }
                        promoProductBlock.IsChecked = true;
                    }
                    NumberOfCheck = FilterPromoProductBlocks.Count;
                }
                else
                {
                    if (NumberOfCheck == FilterPromoProductBlocks.Count)
                    {
                        foreach (PromoProductBlockViewModel promoProductBlock in FilterPromoProductBlocks)
                        {
                            if (promoProductBlock.IsChecked)
                            {
                                TotalProducts--;
                                SelectedPromoProductBlocks.Remove(promoProductBlock);
                            }
                            promoProductBlock.IsChecked = false;
                        }
                    }
                    NumberOfCheck = 0;
                }
            }
        }
        private int numberOfCheck;
        public int NumberOfCheck
        {
            get { return numberOfCheck; }
            set
            {
                numberOfCheck = value;
                OnPropertyChanged();
                if (numberOfCheck == FilterPromoProductBlocks.Count)
                {
                    IsCheckedAll = true;
                }
                else
                {
                    IsCheckedAll = false;
                }
            }
        }
        private int totalProducts;
        public int TotalProducts
        {
            get { return totalProducts; }
            set
            {
                totalProducts = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<PromoProductBlockViewModel> allPromoProductBlocks;
        public ObservableCollection<PromoProductBlockViewModel> AllPromoProductBlocks
        {
            get { return allPromoProductBlocks; }
            set
            {
                allPromoProductBlocks = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<PromoProductBlockViewModel> filterPromoProductBlocks;
        public ObservableCollection<PromoProductBlockViewModel> FilterPromoProductBlocks
        {
            get { return filterPromoProductBlocks; }
            set
            {
                filterPromoProductBlocks = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<PromoProductBlockViewModel> selectedPromoProductBlocks;
        public ObservableCollection<PromoProductBlockViewModel> SelectedPromoProductBlocks
        {
            get { return selectedPromoProductBlocks; }
            set
            {
                selectedPromoProductBlocks = value;
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
        public AddNewProductPromoViewModel(Models.Promo promo)
        {
            SelectedPromo = promo;
            SelectedPromoProductBlocks = new ObservableCollection<PromoProductBlockViewModel>();
            AllPromoProductBlocks = new ObservableCollection<PromoProductBlockViewModel>();
            CompletedCommand = new RelayCommandWithNoParameter(() =>
            {
                var closeDialog = DialogHost.CloseDialogCommand;
                closeDialog.Execute(SelectedPromoProductBlocks, null);
            });
            CancelCommand = new RelayCommandWithNoParameter(() =>
            {
                var closeDialog = DialogHost.CloseDialogCommand;
                closeDialog.Execute(null, null);
            });
            ClickProductCommand = new RelayCommand<object>((p) => p != null, (p) =>
            {
                PromoProductBlockViewModel promoProductBlockViewModel = p as PromoProductBlockViewModel;
                if (promoProductBlockViewModel != null)
                {
                    promoProductBlockViewModel.IsChecked = !promoProductBlockViewModel.IsChecked;
                    if (promoProductBlockViewModel.IsChecked)
                    {
                        SelectedPromoProductBlocks.Insert(0, promoProductBlockViewModel);
                        NumberOfCheck++;
                        TotalProducts++;
                    }
                    else
                    {
                        SelectedPromoProductBlocks.Remove(promoProductBlockViewModel);
                        NumberOfCheck--;
                        TotalProducts--;
                    }
                }
            });
            ResetCommand = new RelayCommandWithNoParameter(() =>
            {
                SearchBy = "Id";
                SearchByValue = "";
                SelectedBrand = null;
                SelectedCategory = null;
                IsShowSelectedProductOnly = true;
            });
            SearchCommand = new RelayCommandWithNoParameter(() =>
            {
                Search();
            });
            Task t = Task.Run(async () =>
            {
                await LoadBrands();
                await LoadCategories();
                await LoadProducts();
            });
            while (!t.IsCompleted) ;
            FilterPromoProductBlocks = new ObservableCollection<PromoProductBlockViewModel>(AllPromoProductBlocks);
            TotalProducts = SelectedPromoProductBlocks.Count();
            IsShowSelectedProductOnly = false;
            if (string.IsNullOrEmpty(SearchBy))
            {
                SearchBy = "Id";
            }
        }
        private async Task LoadBrands()
        {
            GenericDataRepository<Models.Brand> brandRepository = new GenericDataRepository<Models.Brand>();
            Brands = new ObservableCollection<Models.Brand>(await brandRepository.GetListAsync(b => b.Status == "NotBanned"));
        }
        private async Task LoadCategories()
        {
            GenericDataRepository<Models.Category> categoryRepository = new GenericDataRepository<Models.Category>();
            Categories = new ObservableCollection<Models.Category>(await categoryRepository.GetListAsync(c => c.Status == "NotBanned"));
        }
        private async Task LoadProducts()
        {
            GenericDataRepository<Models.Product> productRepository = new GenericDataRepository<Models.Product>();
            ObservableCollection<Models.Product> allProduct = new ObservableCollection<Models.Product>(await productRepository.GetListAsync(p => (p.Status == "NotBanned" && p.IdShop == SelectedPromo.IdShop), 
                                                                                                                                            p => p.Brand,
                                                                                                                                            p => p.Category,
                                                                                                                                            p => p.ImageProducts));
            foreach(Models.Product product in allProduct) 
            {
                PromoProductBlockViewModel promoProductBlockViewModel = new PromoProductBlockViewModel(product);
                AllPromoProductBlocks.Add(promoProductBlockViewModel);
                if(SelectedPromo.Products.Any(p=>p.Id == product.Id))
                {
                    SelectedPromoProductBlocks.Add(promoProductBlockViewModel);
                    promoProductBlockViewModel.IsChecked = true;
                }    
            }
        }
        private void Search()
        {
            if(IsShowSelectedProductOnly)
            {
                if(SearchBy == "Id") 
                {
                    FilterPromoProductBlocks = new ObservableCollection<PromoProductBlockViewModel>(SelectedPromoProductBlocks.Where(p => (p.SelectedProduct.Id.Contains(SearchByValue??"") &&
                                                                                                                                            ((SelectedCategory == null) ? true : (p.SelectedProduct.IdCategory == SelectedCategory.Id)) &&
                                                                                                                                            ((SelectedBrand == null) ? true : (p.SelectedProduct.IdBrand == SelectedBrand.Id)))));
                }
                else if(SearchBy == "Product Name")
                {
                    FilterPromoProductBlocks = new ObservableCollection<PromoProductBlockViewModel>(SelectedPromoProductBlocks.Where(p => (p.SelectedProduct.Name.Contains(SearchByValue ?? "") &&
                                                                                                                                            ((SelectedCategory == null) ? true : (p.SelectedProduct.IdCategory == SelectedCategory.Id)) &&
                                                                                                                                            ((SelectedBrand == null) ? true : (p.SelectedProduct.IdBrand == SelectedBrand.Id)))));
                }
            }
            else
            {
                if (SearchBy == "Id")
                {
                    FilterPromoProductBlocks = new ObservableCollection<PromoProductBlockViewModel>(AllPromoProductBlocks.Where(p => (p.SelectedProduct.Id.Contains(SearchByValue ?? "") &&
                                                                                                                                            ((SelectedCategory == null) ? true : (p.SelectedProduct.IdCategory == SelectedCategory.Id)) &&
                                                                                                                                            ((SelectedBrand == null) ? true : (p.SelectedProduct.IdBrand == SelectedBrand.Id)))));
                }
                else if (SearchBy == "Product Name")
                {
                    FilterPromoProductBlocks = new ObservableCollection<PromoProductBlockViewModel>(AllPromoProductBlocks.Where(p => (p.SelectedProduct.Name.Contains(SearchByValue ?? "") &&
                                                                                                                                            ((SelectedCategory == null) ? true : (p.SelectedProduct.IdCategory == SelectedCategory.Id)) &&
                                                                                                                                            ((SelectedBrand == null) ? true : (p.SelectedProduct.IdBrand == SelectedBrand.Id)))));
                }
            }
            NumberOfCheck = 0;
            int count = 0;
            foreach (PromoProductBlockViewModel promoProductBlockViewModel in FilterPromoProductBlocks)
            {
                if (promoProductBlockViewModel.IsChecked)
                {
                    count++;
                }
            }
            NumberOfCheck = count;
        }
    }
}