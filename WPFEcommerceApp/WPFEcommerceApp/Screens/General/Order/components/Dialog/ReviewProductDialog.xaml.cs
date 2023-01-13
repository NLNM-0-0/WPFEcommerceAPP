using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
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
using DataAccessLayer;
using WPFEcommerceApp.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WPFEcommerceApp {
    /// <summary>
    /// Interaction logic for ReviewProductDialog.xaml
    /// </summary>
    public partial class ReviewProductDialog : UserControl, INotifyPropertyChanged {


        private int _value;

        public int Value {
            get { return _value; }
            set { _value = value; OnPropertyChanged(); }
        }

        private ICommand onOK;

        public ICommand OnOK {
            get { return onOK; }
            set { onOK = value; OnPropertyChanged(); }
        }

        public List<ReviewProduct> ProductList {
            get { return (List<ReviewProduct>)GetValue(ProductListProperty); }
            set { SetValue(ProductListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Param.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProductListProperty =
            DependencyProperty.Register("ProductList", typeof(List<ReviewProduct>), typeof(ReviewProductDialog), new PropertyMetadata(null));


        readonly GenericDataRepository<Rating> ratingRepo = new GenericDataRepository<Rating>();
        readonly GenericDataRepository<OrderInfo> orderInfoRepo = new GenericDataRepository<OrderInfo>();

        public ReviewProductDialog() {
            InitializeComponent();

            OnOK = new RelayCommand<object>(p => true, async p => {
                List<ReviewProduct> t = new List<ReviewProduct>(ProductList);
                for(int i = 0; i < ProductList.Count; i++) {
                    var tmp = new Rating();
                    string id = await GenerateID.Gen(typeof(Rating));
                    tmp.Id = id;
                    tmp.DateRating = DateTime.Now;
                    tmp.Rating1 = ProductList[i].Rating;
                    tmp.Comment = ProductList[i].Comment;
                    await ratingRepo.Add(tmp);

                    var oi = await orderInfoRepo.GetSingleAsync(d => {
                        return d.IdOrder == t[i].IdOrder &&
                        d.IdProduct == t[i].Product.ID && d.Size == t[i].Product.Size;
                    });

                    oi.IdRating = id;
                    await orderInfoRepo.Update(oi);
                }
                var od = OrderStore.instance.GetOrder(t[0].IdOrder);
                od.Status = "Completed";
                await OrderStore.instance.Update(od);
            });
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
