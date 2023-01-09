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
    /// Interaction logic for PromoInformation.xaml
    /// </summary>
    public partial class PromoInformation : UserControl
    {
        Models.Promo promo = new Models.Promo();
        public PromoInformation()
        {
            Task.Run(async () => await Load()).Wait();
            this.DataContext = new PromoInformationViewModel(promo);
            InitializeComponent();
        }
        public async Task Load()
        {
            GenericDataRepository<Models.Promo> genericDataRepository= new GenericDataRepository<Models.Promo>();
            promo = await genericDataRepository.GetSingleAsync(p=>p.Id == "8##QZd6w", 
                                                                p => p.Products,
                                                                p => p.MUser,
                                                                p => p.Products.Select(temp => temp.ImageProducts),
                                                                p => p.Products.Select(temp => temp.Brand),
                                                                p => p.Products.Select(temp => temp.Category));
        }
    }
}
