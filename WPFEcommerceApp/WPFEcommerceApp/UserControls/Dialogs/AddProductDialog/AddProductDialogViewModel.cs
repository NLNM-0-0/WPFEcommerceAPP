using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFEcommerceApp.Models;
using WPFEcommerceApp.UserControls.Dialogs.AddProductDialog;

namespace WPFEcommerceApp
{
    internal class AddProductDialogViewModel : BaseViewModel
    {
        public ICommand AddSizeCommand { get; set; }
        public ICommand DeleteSizeCommand { get; set; }
        public ICommand RequestProductCommand { get; set; }
        public ICommand OpenAddBrandDialogCommand { get; set; }
        public ICommand OpenAddCategoryDialogCommand { get; set; }
        public ICommand OpenChangeImageDialogCommand { get; set; }
        private Models.Product registerProduct;
        public Models.Product RegisterProduct
        {
            get => registerProduct;
            set
            {
                registerProduct = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<string> imageProducts;
        public ObservableCollection<string> ImageProducts
        {
            get => imageProducts;
            set
            {
                imageProducts = value;
                OnPropertyChanged();
            }
        }
        public AddProductDialogViewModel()
        {
            RegisterProduct = new Models.Product();
            ImageProducts = new ObservableCollection<string>();  
            AddSizeCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                ComboBoxItem comboBoxItem = (ComboBoxItem)((p as System.Windows.Controls.ComboBox).SelectedItem);

                if (comboBoxItem != null)
                {
                    if (comboBoxItem.Content.ToString() == "S")
                    {
                        RegisterProduct.IsHadSizeS = true;
                    }
                    else if (comboBoxItem.Content.ToString() == "M")
                    {
                        RegisterProduct.IsHadSizeM = true;
                    }
                    else if (comboBoxItem.Content.ToString() == "L")
                    {
                        RegisterProduct.IsHadSizeL = true;
                    }
                    else if (comboBoxItem.Content.ToString() == "XL")
                    {
                        RegisterProduct.IsHadSizeXL = true;
                    }
                    else if (comboBoxItem.Content.ToString() == "XXL")
                    {
                        RegisterProduct.IsHadSizeXXL = true;
                    }
                }
            });

            DeleteSizeCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                SizeBlock sizeBlock = p as SizeBlock;
                string size = sizeBlock.Size.ToString();
                if (size == "S")
                {
                    RegisterProduct.IsHadSizeS = false;
                }
                else if (size == "M")
                {
                    RegisterProduct.IsHadSizeM = false;
                }
                else if (size == "L")
                {
                    RegisterProduct.IsHadSizeL = false;
                }
                else if (size == "XL")
                {
                    RegisterProduct.IsHadSizeXL = false;
                }
                else if (size == "XXL")
                {
                    RegisterProduct.IsHadSizeXXL = false;
                }
            });
            OpenChangeImageDialogCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                ChangeImageProductDialog addBrandDialog = new ChangeImageProductDialog();
                addBrandDialog.DataContext = new ChangeImageProductDialogViewModel(ImageProducts);
                DialogHost.Show(addBrandDialog, "SecondDialog");
            });
            OpenAddBrandDialogCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                AddBrandDialog addBrandDialog = new AddBrandDialog();
                addBrandDialog.DataContext = new AddBrandDialogViewModel();
                DialogHost.Show(addBrandDialog, "SecondDialog");
            });
            OpenAddCategoryDialogCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                AddCategoryDialog addCategoryDialog = new AddCategoryDialog();
                addCategoryDialog.DataContext = new AddCategoryDialogViewModel();
                DialogHost.Show(addCategoryDialog, "SecondDialog");
            });
            RequestProductCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                //Lưu dữ liêu xuống data base

                //Tắt dialogHost
                /*DialogHost.Close(typeof(AddProductDialog));*/
                var closeDialog = DialogHost.CloseDialogCommand;
                closeDialog.Execute(null, null);
            });
        }
    }
}
