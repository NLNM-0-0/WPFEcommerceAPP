using DataAccessLayer;
using System;
using System.Collections.Generic;
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

namespace WPFEcommerceApp
{
    /// <summary>
    /// Interaction logic for ProductDetail.xaml
    /// </summary>
    public partial class ProductDetail : UserControl
    {
        public Models.Product Product { get; set; }
        public ProductDetail()
        {
            InitializeComponent();
            Task task = Task.Run(async () => await Load());
            while (!task.IsCompleted) ;
            this.DataContext = new ProductDetailViewModel(Product);
        }
        public async Task Load()
        {
            GenericDataRepository<Models.Product> genericDataRepository = new GenericDataRepository<Models.Product>();
            Product = await genericDataRepository.GetSingleAsync(p => p.Id == 1,
                                                             p => p.Brand,
                                                              p => p.Category,
                                                              p => p.MUser,
                                                              p => p.ImageProducts);
        }
    }
}

