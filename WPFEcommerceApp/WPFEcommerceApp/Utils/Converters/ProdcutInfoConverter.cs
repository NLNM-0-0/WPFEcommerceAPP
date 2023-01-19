using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFEcommerceApp
{
    public class ProductInfoConverter : IMultiValueConverter
    {
        private enum productInfo
        {
            Status,
            Name,
            Price,
            Images,
            IsOneSize,
            IsHadSizeS,
            IsHadSizeM,
            IsHadSizeL,
            IsHadSizeXL,
            IsHadSizeXXL,
            Category,
            Brand,
            InStock,
            Sale,
            Color,
            Description
        }
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                string name = (string)values[(int)productInfo.Name];
                string price = (string)values[(int)productInfo.Price];
                ObservableCollection<MImageProuct> imageProducts = (ObservableCollection<MImageProuct>)values[(int)productInfo.Images];
                bool isOneSize = (bool)values[(int)productInfo.IsOneSize];
                bool isHadSizeS = (bool)values[(int)productInfo.IsHadSizeS];
                bool isHadSizeM = (bool)values[(int)productInfo.IsHadSizeM];
                bool isHadSizeL = (bool)values[(int)productInfo.IsHadSizeL];
                bool isHadSizeXL = (bool)values[(int)productInfo.IsHadSizeXL];
                bool isHadSizeXXL = (bool)values[(int)productInfo.IsHadSizeXXL];
                Models.Category category = (Models.Category)values[(int)productInfo.Category];
                Models.Brand brand = (Models.Brand)values[(int)productInfo.Brand];
                string inStock = (string)values[(int)productInfo.InStock];
                string sale = (string)values[(int)productInfo.Sale];
                string color = (string)values[(int)productInfo.Color];
                string description = (string)values[(int)productInfo.Description];

                Models.Product product = new Models.Product();
                product.Name = name;
                product.Price = double.Parse(price == "" ? "0" : price);
                product.ImageProducts.Clear();
                if (imageProducts != null)
                {
                    foreach (MImageProuct image in imageProducts)
                    {
                        Models.ImageProduct imageProduct = new Models.ImageProduct();
                        imageProduct.Source = image.Source;
                        product.ImageProducts.Add(imageProduct);
                    }
                }
                else
                {
                    product.ImageProducts = new List<Models.ImageProduct>();
                }
                product.IsOneSize = isOneSize;
                product.IsHadSizeS = isHadSizeS;
                product.IsHadSizeM = isHadSizeM;
                product.IsHadSizeL = isHadSizeL;
                product.IsHadSizeXL = isHadSizeXL;
                product.IsHadSizeXXL = isHadSizeXXL;
                product.Category = category;
                product.Brand = brand;
                product.InStock = int.Parse(inStock == "" ? "0" : inStock);
                product.Sale = int.Parse(sale == "" ? "0" : sale);
                product.Color = color;
                product.Description = description;

                return product;
            }
            catch
            { return null; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
