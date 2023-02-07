using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
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
        private double total;
        public double Total
        {
            get
            {
                total = 0;
                foreach (BagBlock b in bags)
                {
                    if (b.IsChecked == true)
                    {
                        total += b.UnitPrice * b.Amount;
                    }
                }
                return total;
            }
        }

        public BagViewModel()
        {
            IsLoadingCheck.IsLoading = 2;
            Task.Run(async () =>
            {
                CartRepo = new GenericDataRepository<Models.Cart>();
                Bags = new ObservableCollection<BagBlock>();
                Plusamount = new RelayCommand<object>((p) => 
                {
                    return p!=null; 
                }, async (p) =>
                {
                    BagBlock bagBlock = p as BagBlock;
                    if (bagBlock.Amount > bagBlock.Product.InStock)
                    {
                        NotificationDialog notificationDialog = new NotificationDialog()
                        {
                            Header = "Oops",
                            ContentDialog = $"There are only {bagBlock.Product.InStock} quantity remaining for this item"
                        };
                        await DialogHost.Show(notificationDialog, "Main");
                    }
                    else
                    {
                        bagBlock.Amount += 1;
                        await UpdateAmount(bagBlock.Product.Id, bagBlock.ProductSize, bagBlock.Amount);
                        OnPropertyChanged(nameof(Total));
                    }
                });
                Tamount = new RelayCommand<object>((p) =>
                {
                    BagBlock bagBlock = p as BagBlock;
                    return bagBlock != null && bagBlock.Amount >= 2;
                }, async (p) =>
                {
                    BagBlock bagBlock = p as BagBlock;
                    bagBlock.Amount -= 1;
                    await UpdateAmount(bagBlock.Product.Id, bagBlock.ProductSize, bagBlock.Amount);
                    OnPropertyChanged(nameof(Total));
                });
                await Load();
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            }).ContinueWith((first) =>
            {
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
                                NumberOfCheck--;
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
                    bool isCanBuy = false;
                    foreach (BagBlock bag in Bags)
                    {
                        if (bag.IsChecked == true && (bag.IsBanned == true || bag.IsOutOfSold))
                            return false;
                        else if(bag.IsChecked)
                        {
                            isCanBuy = true;
                        }
                    }
                    return isCanBuy;
                }, async (p) =>
                {
                    Dictionary<string, Tuple<MUser, List<Product>>> prdList = new Dictionary<string, Tuple<MUser, List<Product>>>();

                    foreach (BagBlock bag in Bags)
                    {
                        if (bag.IsChecked == false) continue;
                        Product temp = new Product(bag.Product, bag.ProductSize, (int)bag.Amount);
                        if (!prdList.ContainsKey(bag.Shop.Id))
                        {
                            prdList[bag.Shop.Id] = new Tuple<MUser, List<Product>>(bag.Shop, new List<Product>());
                        }
                        prdList[bag.Shop.Id].Item2.Add(temp);
                    }

                    List<Order> orderList = new List<Order>();

                    var prdListConvert = prdList.Values.ToList();
                    foreach (var list in prdListConvert)
                    {
                        var id = await GenerateID.Gen(typeof(MOrder));
                        Order o = new Order(list.Item2)
                        {
                            ID = id,
                            IDCustomer = AccountStore.instance.CurrentAccount.Id,
                            IDShop = list.Item1.Id,
                            Status = "Processing",
                            ShipTotal = 0,
                            DateBegin = DateTime.Now,
                            ShopName = list.Item1.Name,
                            ShopImage = list.Item1.SourceImageAva
                        };
                        orderList.Add(o);
                    }
                    DelCommand.Execute(null);
                    NavigateProvider.CheckoutScreen().Navigate(orderList);
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
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            });
        }
        private async Task RemoveBag(string idProduct)
        {
            MainViewModel.SetLoading(true);
            GenericDataRepository<Cart> genericData = new GenericDataRepository<Cart>();
            Models.Cart cart = await genericData.GetSingleAsync(p => p.IdProduct == idProduct && p.IdUser == AccountStore.instance.CurrentAccount.Id);
            await genericData.Remove(cart);
            MainViewModel.SetLoading(false);
        }
        private async Task Load()
        {
            MainViewModel.SetLoading(true);
            var cartList = new ObservableCollection<Models.Cart>((await CartRepo.GetListAsync(item => item.IdUser == AccountStore.instance.CurrentAccount.Id,
                                                        item => item.Product,
                                                        item => item.Product.MUser,
                                                        item => item.Product.ImageProducts
                                                        )).OrderBy(c => c.Product.BanLevel != 0).ThenBy(c => (c.Product.InStock == 0)));

            Bags = new ObservableCollection<BagBlock>(cartList.Select(item => new BagBlock
            {
                Product = item.Product,
                Shop = item.Product.MUser,
                ID = item.IdProduct,
                ProductImage = (item.Product.ImageProducts.Count == 0) ? Properties.Resources.DefaultProductImage : item.Product.ImageProducts.First().Source,
                ProductName = item.Product.Name,
                ShopName = item.Product.MUser.Name,
                ProductSize = item.Size,
                UnitPrice = item.Product.Price,
                Amount = (item.Amount == null || item.Product.BanLevel != 0 || item.Product.InStock == 0) ?0:(int)item.Amount,
                Price = (item.Amount * item.Product.Price) ?? 0,
                Tamount = Tamount,
                Plusamount = Plusamount, 
                IsBanned = (item.Product.BanLevel != 0),
                IsOutOfSold = (item.Product.InStock == 0)
            }));
            MainViewModel.SetLoading(false);
        }
        private async Task UpdateAmount(string idProduct, string size, int amount)
        {
            Models.Cart cart = await CartRepo.GetSingleAsync(c => (c.IdUser == AccountStore.instance.CurrentAccount.Id &&
                                                                c.Size == size));
            cart.Amount = amount;
            await CartRepo.Update(cart);
        }
    }
}

