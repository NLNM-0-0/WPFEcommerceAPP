using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    internal class BagViewModel : BaseViewModel
    {
        public GenericDataRepository<Models.Cart> CartRepo;

        public ICommand CheckAllProductCommand { get; set; }
        public ICommand DelCommand { get; set; }

        public ICommand BuyCommand { get; set; }
        public ICommand Plusamount { get; set; }

        public ICommand Tamount { get; set; }
        public ICommand ClickCommand { get; set; }



        private ObservableCollection<BagBlock> bags;
        public ObservableCollection<BagBlock> Bags
        {
            get { return bags; }
            set
            {
                bags = value;
                OnPropertyChanged();
            }
        }
        /*private bool isCheckedAll = false;
        public bool IsCheckedAll
        {
            get { return isCheckedAll; }

            set
            {
                isCheckedAll = value;
                OnPropertyChanged();
                foreach (BagBlock b in bags)
                {
                    b.IsChecked = isCheckedAll;
                }
                OnPropertyChanged(nameof(Total));
            }
        }*/
        private bool isCheckedAll;
        public bool IsCheckedAll
        {
            get { return isCheckedAll; }
            set
            {
                isCheckedAll = value;
                if (isCheckedAll)
                {
                    foreach (BagBlock b in bags)
                    {
                        b.IsChecked = true;
                    }
                    NumberOfCheck = bags.Count;
                }
                else
                {
                    if (NumberOfCheck == bags.Count)
                    {
                        foreach (BagBlock b in bags)
                        {
                            b.IsChecked = false;
                        }
                        NumberOfCheck = 0;
                    }
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
                if (numberOfCheck == bags.Count)
                {
                    IsCheckedAll = true;
                }
                else
                {
                    IsCheckedAll = false;
                }
                OnPropertyChanged(nameof(Total));
            }
        }
        private long total;
        public long Total
        {
            get
            {
                total = 0;
                foreach (BagBlock b in bags)
                {
                    if (b.IsChecked == true)
                    {
                        total += b.Price * b.Amount;
                    }
                }
                return total;
            }
        }

        public BagViewModel()
        {
            CartRepo = new GenericDataRepository<Models.Cart>();
            Bags = new ObservableCollection<BagBlock>();
            Task task = Task.Run(async () => await Load());
            DelCommand = new RelayCommand<object>((p) =>
            {
                foreach (BagBlock bag in Bags)
                {
                    if (bag.IsChecked == true)
                        return true;
                }
                return false;
            }, (p) =>
            {
                try
                {
                    for (int i = 0; i < Bags.Count(); i++)
                    {
                        if (Bags[i].IsChecked == true)
                        {
                            Task.Run(async () => await RemoveBag(Bags[i].ID)).Wait();
                            Bags.RemoveAt(i);
                            i--;
                        }
                    }
                    OnPropertyChanged(nameof(Total));
                    IsCheckedAll = false;
                }
                catch
                {

                }
            });
            BuyCommand = new RelayCommand<object>((p) =>
            {
                foreach (BagBlock bag in Bags)
                {
                    if (bag.IsChecked == true)
                        return true;
                }
                return false;
            }, (p) =>
            {
                MessageBox.Show("nav to checkout");
            });
            Plusamount = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                BagBlock bagBlock = p as BagBlock;
                bagBlock.Amount += 1;
                OnPropertyChanged(nameof(Total));
            });
            Tamount = new RelayCommand<object>((p) =>
            {
                BagBlock bagBlock = p as BagBlock;
                return bagBlock != null && bagBlock.Amount >= 2;
            }, (p) =>
            {
                BagBlock bagBlock = p as BagBlock;
                bagBlock.Amount -= 1;
                OnPropertyChanged(nameof(Total));
            });
            ClickCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                BagBlock bagBlock = p as BagBlock;
                bagBlock.IsChecked = !bagBlock.IsChecked;
                if (!bagBlock.IsChecked)
                {
                    NumberOfCheck--;
                }
                else
                {
                    NumberOfCheck++;
                }
                OnPropertyChanged(nameof(Total));
            });
        }
        private async Task RemoveBag(string idProduct)
        {
            MainViewModel.IsLoading = true;
            GenericDataRepository<Cart> genericData = new GenericDataRepository<Cart>();
            Models.Cart cart = await genericData.GetSingleAsync(p => p.IdProduct == idProduct && p.IdUser == AccountStore.instance.CurrentAccount.Id);
            await genericData.Remove(cart);
            MainViewModel.IsLoading = false;
        }
        private async Task Load()
        {
            MainViewModel.IsLoading = true;
            var cartList = new ObservableCollection<Models.Cart>(await CartRepo.GetListAsync(item => item.IdUser == AccountStore.instance.CurrentAccount.Id,
                                                        item => item.Product,
                                                        item => item.Product.MUser,
                                                        item => item.Product.ImageProducts
                                                        ));

            Bags = new ObservableCollection<BagBlock>(cartList.Select(item => new BagBlock
            {
                ID = item.IdProduct,
                ProductImage = (item.Product.ImageProducts.Count == 0) ? Properties.Resources.DefaultProductImage : item.Product.ImageProducts.First().Source,
                ProductName = item.Product.Name,
                ShopName = item.Product.MUser.Name,
                ProductSize = item.Size,
                UnitPrice = item.Product.Price,
                Amount = item.Amount ?? 0,
                Price = (item.Amount * item.Product.Price) ?? 0,
                Tamount = Tamount,
                Plusamount = Plusamount
            }));
            MainViewModel.IsLoading = false;
        }
    }
}

