using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Microsoft.Win32;
using System.Diagnostics;

namespace WPFEcommerceApp
{
    /// <summary>
    /// Interaction logic for OrderInfoPdf.xaml
    /// </summary>
    public partial class OrderInfoPdf : UserControl
    {
        public OrderInfoPdf()
        {
            InitializeComponent();
        }
        public void Print()
        {
            if (DataContext != null && DataContext.GetType() == typeof(OrderInfoPdfViewModel))
            {
                try
                {
                    this.IsEnabled = false;
                    PrintDialog printDialog = new PrintDialog();
                    if (printDialog.ShowDialog() == true)
                    { 
                        printDialog.PrintVisual(grid, $"invoice_{(DataContext as OrderInfoPdfViewModel).Order.Id}");
                    }
                }
                finally
                {
                    this.IsEnabled = true;
                }
            }
        }
    }
}
