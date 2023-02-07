using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace WPFEcommerceApp
{
    /// <summary>
    /// Interaction logic for ProductDetailMini.xaml
    /// </summary>
    public partial class ProductDetailMini : UserControl
    {
        public ProductDetailMini()
        {
            InitializeComponent();
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int amount;
            int maxAmount;
            if (int.TryParse((sender as TextBox).Text, out amount))
            {
                if (int.Parse((sender as TextBox).Text) == 0)
                {
                    (sender as TextBox).Text = "1";
                }
                else if (int.TryParse(instockTextBox.Text, out maxAmount))
                {
                    if (maxAmount == 0 && (sender as TextBox).Text == "1")
                    {
                        maxAmount = int.Parse((sender as TextBox).Text);
                    }
                    if (amount > maxAmount)
                    {
                        (sender as TextBox).Text = maxAmount.ToString();
                    }
                }
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private async void HeartGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext != null && this.DataContext.GetType() == typeof(ProductDetailNormalViewModel))
            {
                ProductDetailNormalViewModel viewModel = (ProductDetailNormalViewModel)this.DataContext;
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
