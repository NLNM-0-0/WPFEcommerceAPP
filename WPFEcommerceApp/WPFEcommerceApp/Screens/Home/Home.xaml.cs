using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public GenericDataRepository<Models.Category> CateRepo;
        public Home()
        {
            InitializeComponent();
            this.DataContext = new HomeViewModel();
            /*List<string> listCategory = new List<string>();
            List<string> listBrand = new List<string>();
            listCategory.Add("Clothes");
            listCategory.Add("Shoes");
            listCategory.Add("Coat");
            listCategory.Add("Hoddie");
            listCategory.Add("Men");
            listCategory.Add("Women");
            listCategory.Add("Cap");
            listCategory.Add("Pants");
            listCategory.Add("T-Shirt");
            listBrand.Add("Hii");
            listBrand.Add("Adu");
            listBrand.Add("HiiHuu");
            Category.ItemsSource = listCategory;
            Brand.ItemsSource = listBrand;*/
        }
    }
}
