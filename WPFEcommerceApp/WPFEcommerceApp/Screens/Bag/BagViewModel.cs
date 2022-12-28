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
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    internal class BagViewModel : BaseViewModel
    {
        public GenericDataRepository<Models.Cart> CartRepo;
        private readonly GenericDataRepository<Models.Product> productRepo;

        public ICommand CheckAllProductCommand { get; set; }
        public ICommand DelCommand { get; set; }

        public ICommand BuyCommand { get; set; }

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
        private bool isCheckedAll = false;
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
                        total += b.Price;
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
            CheckAllProductCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                
                if(!IsCheckedAll)
                {
                    bool isCheckedAll = true;
                    foreach (BagBlock bag in Bags)
                    {
                        if (bag.IsChecked == false)
                        {
                            isCheckedAll = false;
                            break;
                        }
                    }
                    if(isCheckedAll==IsCheckedAll)
                    {
                        OnPropertyChanged(nameof(Total));
                    }    
                    IsCheckedAll = isCheckedAll;
                }    
                else
                {
                    isCheckedAll = false;
                    OnPropertyChanged(nameof(IsCheckedAll));
                    OnPropertyChanged(nameof(Total));
                }
            });
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
                    foreach (BagBlock bag in Bags)
                    {
                        if (bag.IsChecked == true)
                        {
                            Task.Run(async () => await RemoveBag(bag.ID));
                            Bags.Remove(bag);
                        }
                    }
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
        }
        private async Task RemoveBag(string idProduct)
        {
            GenericDataRepository<Cart> genericData = new GenericDataRepository<Cart>();
            Models.Cart cart = await genericData.GetSingleAsync(p => p.IdProduct == idProduct && p.IdUser == AccountStore.instance.CurrentAccount.Id);
            await genericData.Remove(cart);
        }
        private async Task Load()
        {
            var cartList = new ObservableCollection<Models.Cart>(await CartRepo.GetListAsync(item => item.IdUser == AccountStore.instance.CurrentAccount.Id,
                                                        item => item.Product,
                                                        item => item.Product.MUser,
                                                        item => item.Product.ImageProducts
                                                        ));

            Bags = new ObservableCollection<BagBlock>(cartList.Select(item => new BagBlock
            {
                ID = item.IdProduct,
                ProductImage = (item.Product.ImageProducts.Count == 0) ? Properties.Resources.DefaultProductImage:item.Product.ImageProducts.First().Source,
                ProductName = item.Product.Name,
                ShopName = item.Product.MUser.Name,
                UnitPrice = item.Product.Price,
                Amount = item.Amount ?? 0,
                Price = (item.Amount * item.Product.Price) ?? 0
            })) ; ; ;
        }
    }
}

