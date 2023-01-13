using DataAccessLayer;
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
        public event Action ClosedDialog;
        private void OnClosedDialog()
        {
            ClosedDialog?.Invoke();
        }
        private GenericDataRepository<Models.Product> productRepository = new GenericDataRepository<Models.Product>();
        private GenericDataRepository<Models.Category> categoryRepository = new GenericDataRepository<Models.Category>();
        private GenericDataRepository<Models.Brand> brandRepository = new GenericDataRepository<Models.Brand>();
        private GenericDataRepository<Models.ImageProduct> imageProductRepository = new GenericDataRepository<Models.ImageProduct>();
        public ICommand AddSizeCommand { get; set; }
        public ICommand DeleteSizeCommand { get; set; }
        public ICommand RequestProductCommand { get; set; }
        public ICommand OpenAddBrandDialogCommand { get; set; }
        public ICommand OpenAddCategoryDialogCommand { get; set; }
        public ICommand OpenChangeImageDialogCommand { get; set; }
        public ICommand CheckOneSizeCommand { get; set; }
        public ICommand KeyDownEnterCommand { get; set; }
        private ObservableCollection<BitmapImage> imageProducts;
        public ObservableCollection<BitmapImage> ImageProducts
        {
            get => imageProducts;
            set
            {
                imageProducts = value;
                OnPropertyChanged();
            }
        }
        private IList<Brand> brands;
        public IList<Brand> Brands
        {
            get
            {
                return brands;
            }
            set
            {
                brands = value;
                OnPropertyChanged();
            }
        }
        private IList<Category> categories;
        public IList<Category> Categories
        {
            get
            {
                return categories;
            }
            set
            {
                categories = value;
                OnPropertyChanged();
            }
        }
        private Brand selectedBrand;
        public Brand SelectedBrand
        {
            get
            {
                return selectedBrand;
            }
            set
            {
                selectedBrand = value;
                OnPropertyChanged();
            }
        }
        private Category selectedCategory;
        public Category SelectedCategory
        {
            get
            {
                return selectedCategory;
            }
            set
            {
                selectedCategory = value;
                OnPropertyChanged();
            }
        }
        private bool isEditting;
        public bool IsEditting
        {
            get => isEditting;
            set
            {
                isEditting = value;
                OnPropertyChanged();
            }
        }
        private bool isBanned;
        public bool IsBanned
        {
            get => isBanned;
            set
            {
                isBanned = value;
                OnPropertyChanged();
            }
        }
        private bool isHadSizeS;
        public bool IsHadSizeS
        {
            get => isHadSizeS;
            set
            {
                isHadSizeS = value;
                OnPropertyChanged();
            }
        }
        private bool isHadSizeM;
        public bool IsHadSizeM
        {
            get => isHadSizeM;
            set
            {
                isHadSizeM = value;
                OnPropertyChanged();
            }
        }
        private bool isHadSizeL;
        public bool IsHadSizeL
        {
            get => isHadSizeL;
            set
            {
                isHadSizeL = value;
                OnPropertyChanged();
            }

        }
        private bool isHadSizeXL;
        public bool IsHadSizeXL
        {
            get => isHadSizeXL;
            set
            {
                isHadSizeXL = value;
                OnPropertyChanged();
            }
        }
        private bool isHadSizeXXL;
        public bool IsHadSizeXXL
        {
            get => isHadSizeXXL;
            set
            {
                isHadSizeXXL = value;
                OnPropertyChanged();
            }
        }
        private bool isHadOneSize;
        public bool IsHadOneSize
        {
            get => isHadOneSize;
            set
            {
                isHadOneSize = value;
                OnPropertyChanged();
            }
        }
        private string productName;
        public string ProductName
        {
            get => productName;
            set
            {
                productName = value;
                OnPropertyChanged();
            }
        }
        private string price;
        public string Price
        {
            get => price;
            set
            {
                price = value;
                OnPropertyChanged();
            }
        }
        private string instock;
        public string InStock
        {
            get => instock;
            set
            {
                instock = value;
                OnPropertyChanged();
            }
        }
        private string sale;
        public string Sale
        {
            get => sale;
            set
            {
                sale = value;
                OnPropertyChanged();
            }
        }
        private string description;
        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged();
            }
        }
        private string color;
        public string Color
        {
            get => color;
            set
            {
                color = value;
                OnPropertyChanged();
            }
        }

        public AddProductDialogViewModel()
        {
            LoadData();
            AddSizeCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                ComboBoxItem comboBoxItem = (ComboBoxItem)((p as System.Windows.Controls.ComboBox).SelectedItem);

                if (comboBoxItem != null)
                {
                    if (comboBoxItem.Content.ToString() == "S")
                    {
                        IsHadSizeS = true;
                    }
                    else if (comboBoxItem.Content.ToString() == "M")
                    {
                        IsHadSizeM = true;
                    }
                    else if (comboBoxItem.Content.ToString() == "L")
                    {
                        IsHadSizeL = true;
                    }
                    else if (comboBoxItem.Content.ToString() == "XL")
                    {
                        IsHadSizeXL = true;
                    }
                    else if (comboBoxItem.Content.ToString() == "XXL")
                    {
                        IsHadSizeXXL = true;
                    }
                }
            });

            DeleteSizeCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                SizeBlock sizeBlock = p as SizeBlock;
                string size = sizeBlock.Size.ToString();
                if (size == "S")
                {
                    IsHadSizeS = false;
                }
                else if (size == "M")
                {
                    IsHadSizeM = false;
                }
                else if (size == "L")
                {
                    IsHadSizeL = false;
                }
                else if (size == "XL")
                {
                    IsHadSizeXL = false;
                }
                else if (size == "XXL")
                {
                    IsHadSizeXXL = false;
                }
            });
            OpenChangeImageDialogCommand = new RelayCommand<object>((p) => { return p != null; }, async (p) =>
            {
                MainViewModel.IsLoading = true;
                ChangeImageProductDialog changeImageProductDialog = new ChangeImageProductDialog();
                changeImageProductDialog.DataContext = new ChangeImageProductDialogViewModel(ImageProducts);
                MainViewModel.IsLoading = false;
                await DialogHost.Show(changeImageProductDialog, "SecondDialog");
            });
            OpenAddBrandDialogCommand = new RelayCommand<object>((p) => { return p != null; }, async (p) =>
            {
                MainViewModel.IsLoading = true;
                AddBrandDialog addBrandDialog = new AddBrandDialog();
                addBrandDialog.DataContext = new AddBrandDialogViewModel();
                MainViewModel.IsLoading = false;
                await DialogHost.Show(addBrandDialog, "SecondDialog");
            });
            OpenAddCategoryDialogCommand = new RelayCommand<object>((p) => { return p != null; }, async (p) =>
            {
                MainViewModel.IsLoading = true;
                AddCategoryDialog addCategoryDialog = new AddCategoryDialog();
                addCategoryDialog.DataContext = new AddCategoryDialogViewModel();
                MainViewModel.IsLoading = false;
                await DialogHost.Show(addCategoryDialog, "SecondDialog");
            });
            RequestProductCommand = new RelayCommand<object>((p) =>
            {
                return !String.IsNullOrEmpty(ProductName) &&
                        SelectedCategory != null &&
                        SelectedBrand != null &&
                        !String.IsNullOrEmpty(Price) &&
                        !String.IsNullOrEmpty(Sale) &&
                        !String.IsNullOrEmpty(InStock) &&
                        !String.IsNullOrEmpty(Color) &&
                        !String.IsNullOrEmpty(Description) &&
                        (IsHadOneSize || IsHadSizeS || IsHadSizeM || IsHadSizeL || IsHadSizeXL || IsHadSizeXXL);
            }, async (p) =>
            {
                var closeDialog = DialogHost.CloseDialogCommand;
                closeDialog.Execute(null, null);
                MainViewModel.IsLoading = true;
                string id = await GenerateID.Gen(typeof(Product));
                await productRepository.Add(new Models.Product()
                {
                    Id = id,
                    Name = ProductName.Trim(),
                    IdShop = AccountStore.instance.CurrentAccount.Id,
                    IdCategory = SelectedCategory.Id,
                    IdBrand = SelectedBrand.Id,
                    Price = long.Parse(Price),
                    Sale = int.Parse(Sale),
                    InStock = int.Parse(InStock),
                    Sold = 0,
                    IsOneSize = IsHadOneSize,
                    IsHadSizeS = this.IsHadSizeS,
                    IsHadSizeM = this.IsHadSizeM,
                    IsHadSizeL = this.IsHadSizeL,
                    IsHadSizeXL = this.IsHadSizeXL,
                    IsHadSizeXXL = this.IsHadSizeXXL,
                    Color = this.Color.Trim(),
                    Description = this.Description.Trim(),
                    DateOfSale = DateTime.Now,
                    //ATCMT
                    BanLevel = 0
                });
                foreach (BitmapImage source in ImageProducts)
                {
                    string link = await FireStorageAPI.PushFromImage(source, "Product", $"{id}");
                    await imageProductRepository.Add(new Models.ImageProduct() { IdProduct = id, Source = link });
                }
                MainViewModel.IsLoading = false;
                OnClosedDialog();
            });
            CheckOneSizeCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                IsHadSizeL = !IsHadOneSize;
                IsHadSizeM = !IsHadOneSize;
                IsHadSizeS = !IsHadOneSize;
                IsHadSizeXL = !IsHadOneSize;
                IsHadSizeXXL = !IsHadOneSize;
            });
            KeyDownEnterCommand = new RelayCommand<object>((p) => p != null, (p) =>
            {
                System.Windows.Controls.Button button = p as System.Windows.Controls.Button;
                if (button.IsEnabled)
                {
                    button.Command.Execute(button);
                }
            });
        }
        public async void LoadData()
        {
            ImageProducts = new ObservableCollection<BitmapImage>();
            IsHadSizeS = false;
            IsHadSizeM = false;
            IsHadSizeL = false;
            IsHadSizeXL = false;
            IsHadSizeXXL = false;
            IsHadOneSize = false;
            await LoadCategories();
            await LoadBrands();
        }
        private async Task LoadCategories()
        {
            Categories = await categoryRepository.GetListAsync(c => c.Status == "NotBanned");
        }
        private async Task LoadBrands()
        {
            Brands = await brandRepository.GetListAsync(b => b.Status == "NotBanned");
        }
    }
}