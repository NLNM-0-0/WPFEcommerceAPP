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
    /// Interaction logic for ProductBlock.xaml
    /// </summary>
    public partial class ProductBlock : UserControl
    {
        public ProductBlock()
        {
            InitializeComponent();
            this.DataContext = new ProductBlockViewModel(this);//đáng lẽ ở đây truyền 1 thể hiện của model mới đúng
                                                               //mà chưa có nên tui ghi tượng trưng thui
        }
    }
}
