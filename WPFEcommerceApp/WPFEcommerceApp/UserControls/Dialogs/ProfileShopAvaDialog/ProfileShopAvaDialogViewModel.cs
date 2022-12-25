using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFEcommerceApp
{
    public class ProfileShopAvaDialogViewModel :BaseViewModel
    {
        public ICommand ChangeAvaShopCommand { get; set; }
        public ICommand ChangeToDefaultAvaShopCommand { get; set; }
        public ICommand SaveAvaShopCommand { get; set; }

        private string sourceImageAva;
        public string SourceImageAva
        {
            get => sourceImageAva;
            set
            {
                sourceImageAva = value;
                OnPropertyChanged();
            }
        }

        public ProfileShopAvaDialogViewModel(Models.MUser shop)
        {
            if (shop == null || shop.SourceImageAva == null)
            {
                SourceImageAva = Properties.Resources.DefaultShopAvaImage;
            }    
            else
            {
                SourceImageAva = shop.SourceImageAva;
            }    
            ChangeAvaShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) => 
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png";
                op.ShowDialog();
                if(op.FileName != "")
                {
                    SourceImageAva = op.FileName;
                }    
            });
            ChangeToDefaultAvaShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                SourceImageAva = Properties.Resources.DefaultShopAvaImage;
            });
            SaveAvaShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                shop.SourceImageAva = SourceImageAva;
                DialogHost.CloseDialogCommand.Execute(null, null);
            });
        }
    }
}
