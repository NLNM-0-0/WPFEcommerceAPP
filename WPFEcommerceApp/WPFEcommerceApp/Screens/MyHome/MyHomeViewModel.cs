
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        public ObservableCollection<ProductBlockViewModel> Products { get; set; }
        public ObservableCollection<ProductBlockViewModel> BestSeller { get; set; }
        public ObservableCollection<ProductBlockViewModel> JustIn { get; set; }

        #endregion

        public ICommand RightCommand { get; set; }
        public ICommand LeftCommand { get; set; }

        public MyHomeViewModel()
        {
            prodRepo = new GenericDataRepository<Models.Product>();
            adsRepo = new GenericDataRepository<Advertisement>();
            RightCommand = new RelayCommand<object>(p => true, Right);
            LeftCommand = new RelayCommand<object>(p => true, Left);
            Task.Run(async () => await Load());
        }

        public async Task Load()
        {
            var products = new List<Models.Product>(await
                prodRepo.GetAllAsync(item => item.Brand,
                item => item.Category,
                item => item.ImageProducts));

            Products = new ObservableCollection<ProductBlockViewModel>(
                products.Take(5).Select(pr => new ProductBlockViewModel(pr)));

            products.Sort(productBySoldDesc);
            BestSeller = new ObservableCollection<ProductBlockViewModel>(
                products.Take(5).Select(pr => new ProductBlockViewModel(pr)));

            products.Sort(productyDateLatest);
            JustIn=new ObservableCollection<ProductBlockViewModel>(
                products.Take(5).Select(pr=>new ProductBlockViewModel(pr)));

            

            var ads = await adsRepo.GetListAsync(item => item.Status == "InUse");
            foreach(var ad in ads)
            {
                if (ad.Position == 1)
                    Banner1 = ad.Image;
                else if (ad.Position == 2)
                    Banner2 = ad.Image;
                else if (ad.Position == 3)
                    Banner3 = ad.Image;
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

        public void Right(object obj)
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
                    var temp3 = (int)((temp + temp2) / 400);
                    var items = listView.ItemsSource.Cast<object>();
                    listView.ScrollIntoView(items.ElementAt(temp3));
                }
                catch { }
                

            }
        }

        public void Left(object obj)
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

                    var temp3 =(int) Math.Round(temp / 410) - 1;
                    if (temp3 == -1)
                        temp3 = 0;
                    var items = listView.ItemsSource.Cast<object>();
                    listView.ScrollIntoView(items.ElementAt(temp3));
                }
                catch { }


            }
        }
    }
}
