
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class MyHomeViewModel : BaseViewModel
    {
        #region Properties
        public string Banner1 { get; set; }
        public string Banner2 { get; set; }
        public string Banner3 { get; set; }


        private GenericDataRepository<Models.Product> prodRepo;
        private GenericDataRepository<Advertisement> adsRepo;
        private GenericDataRepository<AdInUse> adInUseRepo;
        public ObservableCollection<ProductBlockViewModel> Products { get; set; }
        public ObservableCollection<ProductBlockViewModel> BestSeller { get; set; }
        public ObservableCollection<ProductBlockViewModel> JustIn { get; set; }

        #endregion

        public ICommand RightCommand { get; set; }
        public ICommand LeftCommand { get; set; }
        public ICommand ToFilter { get; set; }
        public MyHomeViewModel()
        {
            prodRepo = new GenericDataRepository<Models.Product>();
            adsRepo = new GenericDataRepository<Advertisement>();
            adInUseRepo = new GenericDataRepository<AdInUse>();
            RightCommand = new RelayCommand<object>(p => true, Right);
            LeftCommand = new RelayCommand<object>(p => true, Left);
            ToFilter = new RelayCommand<object>(p => p != null, (p) =>
            {
                var list = p as string;
                FilterStatus option;
                if (list == null)
                    return;
                else if (list == "Best Seller")
                    option = FilterStatus.BestSeller;
                else if (list == "Just In")
                    option = FilterStatus.New;
                else if (list == "More")
                    option = FilterStatus.All;
                else
                    option=FilterStatus.All;

                var temp = new FilterObject("", null, null, null, option);
                NavigateProvider.FilterScreen().Navigate(temp);
            });
            Task.Run(async () => await Load());
        }


        public async Task Load()
        {
            var products = new List<Models.Product>(await
                prodRepo.GetListAsync(item=>item.BanLevel==0 && item.InStock>0,
                item => item.Brand,
                item => item.Category,
                item => item.ImageProducts,
                item => item.MUser));

            products.Sort(productBySoldDesc);
            BestSeller = new ObservableCollection<ProductBlockViewModel>(
                products.Take(5).Select(pr => new ProductBlockViewModel(pr)));

            products.Sort(productyDateLatest);
            JustIn=new ObservableCollection<ProductBlockViewModel>(
                products.Take(5).Select(pr=>new ProductBlockViewModel(pr)));

            Products = new ObservableCollection<ProductBlockViewModel>(
                products.Where(item=>!BestSeller.Any(item2=>item.Id==item2.SelectedProduct.Id)
                && !JustIn.Any(item3=>item.Id==item3.SelectedProduct.Id))
                .Take(5).Select(pr => new ProductBlockViewModel(pr)));

            var ads = await adInUseRepo.GetAllAsync(item => item.Advertisement);
            foreach (var ad in ads)
            {
                if (ad.Position == 1)
                    Banner1 = ad.Advertisement.Image;
                else if (ad.Position == 2)
                    Banner2 = ad.Advertisement.Image;
                else if (ad.Position == 3)
                    Banner3 = ad.Advertisement.Image;
            }

        }

        private int productBySoldDesc(Models.Product x, Models.Product y)
        {
            if (x == null || y == null)
                return 0;

            if (x.Sold > y.Sold)
                return -1;
            else if (x.Sold < y.Sold)
                return 1;
            else
                return 0;
        }

        private int productyDateLatest(Models.Product x, Models.Product y)
        {
            if (x == null || y == null)
                return 0;

            if (x.DateOfSale > y.DateOfSale)
                return -1;
            else if (x.DateOfSale < y.DateOfSale)
                return 1;
            else
                return 0;
        }

        public static void Right(object obj)
        {
            var listView = obj as ListView;
            //ScrollViewer scrollViewer = listView.GetVisualChild<ScrollViewer>();

            // Get the border of the listview (first child of a listview)
            Decorator border = VisualTreeHelper.GetChild(listView, 0) as Decorator;


            // Get scrollviewer
            ScrollViewer scrollViewer = border.Child as ScrollViewer;

            if (scrollViewer != null)
            {
                try
                {
                    var temp = scrollViewer.HorizontalOffset;
                    var temp2 = scrollViewer.ViewportWidth;
                    var temp3 = (int)((temp + temp2) / 350);
                    var items = listView.ItemsSource.Cast<object>();
                    listView.ScrollIntoView(items.ElementAt(temp3));
                }
                catch { }
                

            }
        }

        public static void Left(object obj)
        {
            var listView = obj as ListView;
            //ScrollViewer scrollViewer = listView.GetVisualChild<ScrollViewer>();

            // Get the border of the listview (first child of a listview)
            Decorator border = VisualTreeHelper.GetChild(listView, 0) as Decorator;

            // Get scrollviewer
            ScrollViewer scrollViewer = border.Child as ScrollViewer;

            if (scrollViewer != null)
            {
                try
                {
                    var temp = scrollViewer.HorizontalOffset;
                    var temp2 = scrollViewer.ViewportWidth;

                    int temp3 = (int)Math.Ceiling(temp/365)-1;

                    var items = listView.ItemsSource.Cast<object>();
                    listView.ScrollIntoView(items.ElementAt(temp3));
                }
                catch { }


            }
        }
    }
}
