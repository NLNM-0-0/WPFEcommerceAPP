using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace WPFEcommerceApp
{
    public class ProfileShopBackgroundDialogViewModel : BaseViewModel
    {
        public ICommand ChangeToDefaultBackgroundShopCommand { get; set; }
        public ICommand ChangeBackgroundShopCommand { get; set; }
        public ICommand SaveBackgroundShopCommand { get; set; }
        private string sourceImageBackground;
        public string SourceImageBackground
        {
            get => sourceImageBackground;
            set
            {
                sourceImageBackground = value;
                OnPropertyChanged();
            }
        }
        public ProfileShopBackgroundDialogViewModel(Models.MUser shop)
        {
            if(shop == null || String.IsNullOrEmpty(shop.SourceImageBackground)) 
            {
                SourceImageBackground = Properties.Resources.DefaultShopBackgroundImage;
            }
            else
            {
                SourceImageBackground = shop.SourceImageBackground;
            }    
            ChangeBackgroundShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png";
                op.ShowDialog();
                if (op.FileName != "")
                {
                    SourceImageBackground = op.FileName;
                }
            });
            ChangeToDefaultBackgroundShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                SourceImageBackground = Properties.Resources.DefaultShopBackgroundImage;
            });
            SaveBackgroundShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                shop.SourceImageBackground = SourceImageBackground;
                DialogHost.CloseDialogCommand.Execute(null, null);
            });
        }
    }
}
