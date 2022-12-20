using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFEcommerceApp.Models;
using WPFEcommerceApp.UserControls.Dialogs.AddProductDialog;

namespace WPFEcommerceApp
{
    public class ProductInfoLandscapeViewModel : BaseViewModel
    {
        private readonly AccountStore _accountStore;
        private GenericDataRepository<Category> categoryRepository = new GenericDataRepository<Category>();
        private GenericDataRepository<Brand>brandRepository = new GenericDataRepository<Brand>();
        private GenericDataRepository<Models.Product> productRepository = new GenericDataRepository<Models.Product>();
        public ICommand SaveProductInfoCommand { get; set; }
        public ICommand EditProductInfoCommand { get; set; }
        public ICommand AddSizeCommand { get; set; }
        public ICommand DeleteSizeCommand { get; set; }
        public ICommand ChangeSelectedImageCommand { get; set; }
        public ICommand OpenChangeImageDialogCommand { get; set; }
        public ICommand OpenAddBrandDialogCommand { get; set; }
        public ICommand OpenAddCategoryDialogCommand { get; set; }
        public ICommand ContactCommand { get; set; }
        public ICommand CheckOneSizeCommand { get; set; }
        public ICommand KeyDownEnterCommand { get; set; }

        private Models.Product selectedProduct;
        public Models.Product SelectedProduct
        {
            get => selectedProduct;
            set
            {
                selectedProduct = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<string> imageProducts;
        public ObservableCollection<string> ImageProducts
        {
            get
            {
                return imageProducts;
            }
            set
            {
                imageProducts = value;
                OnPropertyChanged();
            }
        }
        private ChangeImageProductDialogViewModel changeImageProductDialogViewModel;
        public ChangeImageProductDialogViewModel ChangeImageProductDialogViewModel
        {
            get => new ChangeImageProductDialogViewModel(ImageProducts);
            set
            {
                changeImageProductDialogViewModel = value;
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
        public System.Windows.Media.Brush ForegroundStatus
        {
            get
            {
                if (SelectedProduct.Status == "Banned")
                {
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(219, 48, 34));
                }
                else if(SelectedProduct.InStock > 0)
                {
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(42, 169, 82));
                }
                else
                {
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(253, 197, 0));
                } 
                    
            }
        }
        public string Status
        {
            get
            {
                if (SelectedProduct.Status == "Banned")
                {
                    return "Banned";
                }
                else if (SelectedProduct.InStock > 0)
                {
                    return "In Stock";
                }
                else
                {
                    return "Out Of Stock";
                }
            }
        }
        public ProductInfoLandscapeViewModel(AccountStore accountStore, Models.Product product)
        {
            _accountStore = accountStore;
            SelectedProduct = product;
            ImageProducts = new ObservableCollection<string>();
            ImageProducts.CollectionChanged += ImageProducts_CollectionChanged;
            if (SelectedProduct.IsOneSize)
            {
                IsHadOneSize= true;
            }    
            else
            {
                IsHadOneSize= false;
            }    
            Task.Run(async () => { await LoadBrands(); await LoadCategories(); });
            IsEditting = false;
            ImageProducts = new ObservableCollection<string>();
            foreach (var item in SelectedProduct.ImageProducts)
            {
                ImageProducts.Add(item.Source);
            }    
            /*ImageProduct.CollectionChanged += ImageSources_CollectionChanged;*/
            if(SelectedProduct.Status == "Banned")
            {
                IsBanned = true;
            }    
            else
            {
                IsBanned = false;
            }
            if ((bool)SelectedProduct.IsHadSizeS)
            {
                IsHadSizeS = true;
            }
            else
            {
                IsHadSizeS = false;
            }
            if ((bool)SelectedProduct.IsHadSizeL)
            {
                IsHadSizeL = true;
            }
            else
            {
                IsHadSizeL = false;
            }
            if ((bool)SelectedProduct.IsHadSizeM)
            {
                IsHadSizeM = true;
            }
            else
            {
                IsHadSizeM = false;
            }
            if ((bool)SelectedProduct.IsHadSizeXL)
            {
                IsHadSizeXL = true;
            }
            else
            {
                IsHadSizeXL = false;
            }
            if ((bool)SelectedProduct.IsHadSizeXXL)
            {
                IsHadSizeXXL = true;
            }
            else
            {
                IsHadSizeXXL = false;
            }
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
            SaveProductInfoCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                WPFEcommerceApp.Models.Product productTemp = p as WPFEcommerceApp.Models.Product;
                SelectedProduct.Name = productTemp.Name;
                SelectedProduct.Price = productTemp.Price;
                SelectedProduct.ImageProducts.Clear();
                foreach(string imageProductSource in ImageProducts)
                {
                    Models.ImageProduct imageProduct = new Models.ImageProduct() { Source = imageProductSource, IdProduct = SelectedProduct.Id };
                    SelectedProduct.ImageProducts.Add(imageProduct);
                }    
                SelectedProduct.IsHadSizeS = productTemp.IsHadSizeS;
                SelectedProduct.IsHadSizeM = productTemp.IsHadSizeM;
                SelectedProduct.IsHadSizeL = productTemp.IsHadSizeL;
                SelectedProduct.IsHadSizeXL = productTemp.IsHadSizeXL;
                SelectedProduct.IsHadSizeXXL = productTemp.IsHadSizeXXL;
                SelectedProduct.Category = productTemp.Category;
                selectedProduct.Brand = productTemp.Brand;
                selectedProduct.InStock = productTemp.InStock;
                selectedProduct.Sold = productTemp.Sold;
                SelectedProduct.Sale = productTemp.Sale;
                SelectedProduct.Description = productTemp.Description;
                SelectedProduct.Color = productTemp.Description;
                IsEditting = false;
                product = SelectedProduct;
                Task t = Task.Run(async () => { await UpdateImageProduct(); await UpdateProduct(); });
                product = SelectedProduct;
                while (!t.IsCompleted) ;
                var closeDialog = DialogHost.CloseDialogCommand;
                closeDialog.Execute(null, null);
            });

            EditProductInfoCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                IsEditting = true;
            });
            OpenChangeImageDialogCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                ChangeImageProductDialog changeImageProductDialog = new ChangeImageProductDialog();
                changeImageProductDialog.DataContext = new ChangeImageProductDialogViewModel(ImageProducts);
                DialogHost.Show(changeImageProductDialog, "SecondDialog");
            });
            OpenAddBrandDialogCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                AddBrandDialog addBrandDialog = new AddBrandDialog();
                addBrandDialog.DataContext = new AddBrandDialogViewModel(_accountStore);
                DialogHost.Show(addBrandDialog, "SecondDialog");
            });
            OpenAddCategoryDialogCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                AddCategoryDialog addCategoryDialog = new AddCategoryDialog();
                addCategoryDialog.DataContext = new AddCategoryDialogViewModel(_accountStore);
                DialogHost.Show(addCategoryDialog, "SecondDialog");
            });
            ContactCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                OpenContact();
            });
            CheckOneSizeCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                IsHadSizeL = !IsHadOneSize;
                IsHadSizeM = !IsHadOneSize;
                IsHadSizeS = !IsHadOneSize;
                IsHadSizeXL = !IsHadOneSize;
                IsHadSizeXXL = !IsHadOneSize;
            });
            KeyDownEnterCommand = new RelayCommand<object>(p => p != null, p =>
            {
                 if(IsBanned)
                 {
                    OpenContact();
                 }  
                 else
                 {
                    Button button = p as Button;
                    if(!IsEditting)
                    {
                        IsEditting = true;
                    }    
                    else if(IsEditting && button.IsEnabled)
                    {
                        button.Command.Execute(button.CommandParameter);
                        IsEditting = false;
                    }    
                 }    
            });
        }
        private void OpenContact()
        {
            NotificationDialog notificationDialog = new NotificationDialog();
            notificationDialog.Header = "Contact Info";
            notificationDialog.ContentDialog = "Please contact us with phone number 0585885214 or email 21520339@uit.gm.edu.vn.";
            DialogHost.Show(notificationDialog, "SecondDialog");
        }

        private void ImageProducts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ImageProducts));
        }

        private async Task LoadCategories()
        {
            Categories = await categoryRepository.GetListAsync(c => c.Status == "NotBanned");
        }
        private async Task LoadBrands()
        {
            Brands = await brandRepository.GetListAsync(b => b.Status == "NotBanned");
        }
        private async Task UpdateProduct()
        {
            Models.Product product = await productRepository.GetSingleAsync(p => p.Id == SelectedProduct.Id);
            product.Name = SelectedProduct.Name;
            product.Price = SelectedProduct.Price;
            product.IsHadSizeS = SelectedProduct.IsHadSizeS;
            product.IsHadSizeM = SelectedProduct.IsHadSizeM;
            product.IsHadSizeL = SelectedProduct.IsHadSizeL;
            product.IsHadSizeXL = SelectedProduct.IsHadSizeXL;
            product.IsHadSizeXXL = SelectedProduct.IsHadSizeXXL;
            product.IdCategory = SelectedProduct.Category.Id;
            product.IdBrand = SelectedProduct.Brand.Id;
            product.InStock = SelectedProduct.InStock;
            product.Sold = SelectedProduct.Sold;
            product.Sale = SelectedProduct.Sale;
            product.Description = SelectedProduct.Description;
            product.Color = SelectedProduct.Color;
            await productRepository.Update(product);
        }
        private async Task UpdateImageProduct()
        {
            var repository = new GenericDataRepository<Models.ImageProduct>();
            ICollection<Models.ImageProduct> imageproducts = await repository.GetListAsync(p => p.IdProduct == SelectedProduct.Id);
            if (imageproducts != null)
            {
                foreach (var imageproduct in imageproducts)
                {
                    await repository.Remove(imageproduct);
                }
            }
            if (SelectedProduct.ImageProducts != null)
            {
                foreach (Models.ImageProduct imageproduct in SelectedProduct.ImageProducts)
                {
                    await repository.Add(imageproduct);
                }
            }
        }
    }
}
