using MaterialDesignThemes.Wpf;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    /// <summary>
    /// Interaction logic for ProductBlock.xaml
    /// </summary>
    public partial class ProductBlock : UserControl
    {
        public ProductBlock()
        {
            InitializeComponent();
        }
        private async void HeartGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(this.DataContext!= null && this.DataContext.GetType() == typeof(ProductBlockViewModel))
            {
                ProductBlockViewModel viewModel = (ProductBlockViewModel)this.DataContext;
                Storyboard favourite = this.Resources["ClickFavouriteProduct"] as Storyboard;
                Storyboard unFavourite = this.Resources["ClickUnFavouriteProduct"] as Storyboard;
                viewModel.IsFavorite = !viewModel.IsFavorite;
                if (viewModel.IsFavorite != null && viewModel.IsFavorite == true)
                {
                    if (favourite != null)
                    {
                        unFavourite.SkipToFill();
                        favourite.Begin();
                    }
                    await viewModel.UpdateFavoriteProduct();
                }  
                else if (viewModel.IsFavorite != null && viewModel.IsFavorite == false)
                {
                    if (unFavourite != null)
                    {
                        favourite.SkipToFill();
                        unFavourite.Begin();
                    }
                    await viewModel.UpdateFavoriteProduct();
                }
            }    
        }
    }
}
