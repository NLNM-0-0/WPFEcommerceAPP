using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFEcommerceApp
{
    public class ShopMainViewModel:BaseViewModel
    {
        public ICommand ChangeAvaShopCommand { get; set; }
        public ICommand ChangeToDefaultAvaShopCommand { get; set; }
        public ICommand SaveAvaShopCommand { get; set; }

        public ICommand ChangeBackgroundShopCommand { get; set; }
        public ICommand ChangeToDefaultBackgroundShopCommand { get; set; }
        public ICommand SaveBackgroundShopCommand { get; set; }

        public ICommand SaveProfileShopCommand { get; set; }
        public ICommand EditProfileShopCommand { get; set; }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        private string phoneNumber;
        public string PhoneNumber
        {
            get
            {
                return phoneNumber;
            }
            set
            {
                phoneNumber = value;
                OnPropertyChanged();
            }
        }
        private string email;
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                email = value;
                OnPropertyChanged();
            }
        }
        private string address;
        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
                OnPropertyChanged();
            }
        }
        private string description;
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                OnPropertyChanged();
            }
        }
        private ImageSource sourceImageAva;
        public ImageSource SourceImageAva
        {
            get
            {
                return sourceImageAva;
            }
            set
            {
                sourceImageAva = value;
                OnPropertyChanged();
            }
        }
        private ImageSource sourceImageBackground;
        public ImageSource SourceImageBackground
        {
            get
            {
                return sourceImageBackground;
            }
            set
            {
                sourceImageBackground = value;
                OnPropertyChanged();
            }
        }
        public ShopMainViewModel(Shop shop)
        {
            Name = shop.Name;
            PhoneNumber = shop.PhoneNumber;
            Email = shop.Email;
            Address = shop.Address;
            Description = shop.Description;
            //SourceImageAva = shop.SourceImageAva;
            //SourceImageBackground = shop.SourceImageBackground;

            ChangeAvaShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) => 
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png";
                op.ShowDialog();
                if(op.FileName != "")
                {
                    SourceImageAva = new BitmapImage(new Uri(op.FileName));
                }    
            });
            ChangeToDefaultAvaShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                //SourceImageAva = Shop.DefaultSourceImageAva;
            });
            SaveAvaShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                DialogHost.CloseDialogCommand.Execute(null, null);
            });

            ChangeBackgroundShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png";
                op.ShowDialog();
                if (op.FileName != "")
                {
                    SourceImageBackground = new BitmapImage(new Uri(op.FileName));
                }
            });
            ChangeToDefaultBackgroundShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                SourceImageBackground = Shop.DefaultSourceImageBackground;
            });
            SaveBackgroundShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                DialogHost.CloseDialogCommand.Execute(null, null);
            });
            SaveProfileShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                (p as System.Windows.Controls.Button).IsEnabled = true;
            });
            EditProfileShopCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                (p as System.Windows.Controls.Button).IsEnabled = false;
            });
        }
    }
}
