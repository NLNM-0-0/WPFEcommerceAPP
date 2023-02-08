using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        public event Action ProductImageChanged;
        private void OnProductImageChange()
        {
            ProductImageChanged?.Invoke();
        }
        private GenericDataRepository<Category> categoryRepository = new GenericDataRepository<Category>();
        private GenericDataRepository<Brand> brandRepository = new GenericDataRepository<Brand>();
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
        public ICommand DoubleClickCommand { get; set; } = new RelayCommandWithNoParameter(() => { });

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
        private ObservableCollection<MImageProuct> imageProducts;
        public ObservableCollection<MImageProuct> ImageProducts
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
        private ObservableCollection<Brand> brands;
        public ObservableCollection<Brand> Brands
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
        private ObservableCollection<Category> categories;
        public ObservableCollection<Category> Categories
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
                if (SelectedProduct.BanLevel != 0)
                {
                    return new SolidColorBrush(System.Windows.Media.Color.FromRgb(219, 48, 34));
                }
                else if (SelectedProduct.InStock > 0)
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
                if (SelectedProduct.BanLevel != 0)
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
        private System.Windows.Controls.UserControl PreviousItem;
        public ProductInfoLandscapeViewModel(Models.Product product)
        {
            SelectedProduct = product;
            ImageProducts = new ObservableCollection<MImageProuct>();
            if (SelectedProduct.ImageProducts.Count != 0)
            {
                foreach (var item in SelectedProduct.ImageProducts)
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(item.Source);
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    ImageProducts.Add(new MImageProuct() { BMImage = bitmapImage, Source = item.Source });
                }
            }
            ImageProducts.CollectionChanged += ImageProducts_CollectionChanged;
            Task.Run(async () =>
            {
                await LoadBrands();
                await LoadCategories();
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            }).ContinueWith((third) =>
            {
                if (SelectedProduct.IsOneSize)
                {
                    IsHadOneSize = true;
                }
                else
                {
                    IsHadOneSize = false;
                }
                IsEditting = false;
                if (SelectedProduct.BanLevel != 0)
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
                SaveProductInfoCommand = new RelayCommand<object>((p) => { return p != null; }, async (p) =>
                {
                    var closeDialog = DialogHost.CloseDialogCommand;
                    closeDialog.Execute(null, null);
                    MainViewModel.SetLoading(true);
                    WPFEcommerceApp.Models.Product productTemp = p as WPFEcommerceApp.Models.Product;
                    SelectedProduct.Name = productTemp.Name.Trim();
                    SelectedProduct.Price = productTemp.Price;
                    SelectedProduct.IsHadSizeS = productTemp.IsHadSizeS;
                    SelectedProduct.IsHadSizeM = productTemp.IsHadSizeM;
                    SelectedProduct.IsHadSizeL = productTemp.IsHadSizeL;
                    SelectedProduct.IsHadSizeXL = productTemp.IsHadSizeXL;
                    SelectedProduct.IsHadSizeXXL = productTemp.IsHadSizeXXL;
                    SelectedProduct.IsOneSize = productTemp.IsOneSize;
                    SelectedProduct.Category = productTemp.Category;
                    SelectedProduct.IdCategory = productTemp.Category.Id;
                    selectedProduct.Brand = productTemp.Brand;
                    SelectedProduct.IdBrand = productTemp.Brand.Id;
                    selectedProduct.InStock = productTemp.InStock;
                    selectedProduct.Sold = productTemp.Sold;
                    SelectedProduct.Sale = productTemp.Sale;
                    SelectedProduct.Description = productTemp.Description.Trim();
                    SelectedProduct.Color = productTemp.Color.Trim();
                    IsEditting = false;
                    product = SelectedProduct;
                    await UpdateImageProduct();
                    await UpdateProduct();
                    product = SelectedProduct;
                    OnProductImageChange();
                    SearchItemViewModel searchItemViewModel = HeaderViewModel.AllItems.Where(hvm => (hvm.IsProduct && (hvm.Model as Models.Product).Id == SelectedProduct.Id)).ElementAt(0);
                    searchItemViewModel.Model = SelectedProduct;
                    searchItemViewModel.Name = SelectedProduct.Name;
                    searchItemViewModel.SourceImage = (SelectedProduct.ImageProducts.Count == 0) ? null : SelectedProduct.ImageProducts.ElementAt(0).Source;
                    MainViewModel.SetLoading(false);
                });

                EditProductInfoCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
                {
                    IsEditting = true;
                });
                OpenChangeImageDialogCommand = new RelayCommand<object>((p) => { return p != null; }, async (p) =>
                {
                    PreviousItem = MainViewModel.UpdateDialog("Main");
                    MainViewModel.SetLoading(true);
                    ChangeImageProductDialog changeImageProductDialog = new ChangeImageProductDialog();
                    changeImageProductDialog.DataContext = new ChangeImageProductDialogViewModel(ImageProducts)
                    {
                        CloseDialogCommand = new RelayCommandWithNoParameter(() =>
                        {
                            DialogHost.CloseDialogCommand.Execute(null, null);
                            if (PreviousItem != null)
                            {
                                DialogHost.Show(PreviousItem, "Main");
                            }
                        })
                    };
                    MainViewModel.SetLoading(false);
                    await DialogHost.Show(changeImageProductDialog, "Main");
                });
                OpenAddBrandDialogCommand = new RelayCommand<object>((p) => { return p != null; }, async (p) =>
                {
                    PreviousItem = MainViewModel.UpdateDialog("Main");
                    MainViewModel.SetLoading(true);
                    AddBrandDialog addBrandDialog = new AddBrandDialog();
                    addBrandDialog.DataContext = new AddBrandDialogViewModel(PreviousItem);
                    MainViewModel.SetLoading(false);
                    await DialogHost.Show(addBrandDialog, "Main");
                });
                OpenAddCategoryDialogCommand = new RelayCommand<object>((p) => { return p != null; }, async (p) =>
                {
                    PreviousItem = MainViewModel.UpdateDialog("Main");
                    MainViewModel.SetLoading(true);
                    AddCategoryDialog addCategoryDialog = new AddCategoryDialog();
                    addCategoryDialog.DataContext = new AddCategoryDialogViewModel(PreviousItem);
                    MainViewModel.SetLoading(false);
                    await DialogHost.Show(addCategoryDialog, "Main");
                });
                ContactCommand = new RelayCommand<object>((p) => { return p != null; }, async (p) =>
                {
                    await OpenContact();
                });
                CheckOneSizeCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
                {
                    IsHadSizeL = !IsHadOneSize;
                    IsHadSizeM = !IsHadOneSize;
                    IsHadSizeS = !IsHadOneSize;
                    IsHadSizeXL = !IsHadOneSize;
                    IsHadSizeXXL = !IsHadOneSize;
                });
                KeyDownEnterCommand = new RelayCommand<object>(p => p != null, async (p) =>
                {
                    if (IsBanned)
                    {
                        await OpenContact();
                    }
                    else
                    {
                        Button button = p as Button;
                        if (!IsEditting)
                        {
                            IsEditting = true;
                        }
                        else if (IsEditting && button.IsEnabled)
                        {
                            button.Command.Execute(button.CommandParameter);
                            IsEditting = false;
                        }
                    }
                });
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    lock (IsLoadingCheck.IsLoading as object)
                    {
                        IsLoadingCheck.IsLoading--;
                    }
                }));
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        private async Task OpenContact()
        {
            PreviousItem = MainViewModel.UpdateDialog("Main");
            MainViewModel.SetLoading(true);
            NotificationDialog notificationDialog = new NotificationDialog();
            notificationDialog.Header = "Contact Info";
            notificationDialog.ContentDialog = $"Please contact us with phone number {Properties.Resources.PhoneNumber} or email {Properties.Resources.Email}.";
            notificationDialog.CloseCommand = new RelayCommandWithNoParameter(() =>
            {
                if (PreviousItem != null)
                {
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    DialogHost.Show(PreviousItem, "Main");
                }
            });
            MainViewModel.SetLoading(false);
            await DialogHost.Show(notificationDialog, "Main");
        }

        private void ImageProducts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ImageProducts));
        }

        private async Task LoadCategories()
        {
            Categories = new ObservableCollection<Models.Category>(await categoryRepository.GetListAsync(c => c.Status == "NotBanned"));
        }
        private async Task LoadBrands()
        {
            Brands = new ObservableCollection<Models.Brand>(await brandRepository.GetListAsync(b => b.Status == "NotBanned"));
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
            SelectedProduct.ImageProducts.Clear();
            var repository = new GenericDataRepository<Models.ImageProduct>();
            ICollection<Models.ImageProduct> imageproducts = await repository.GetListAsync(p => p.IdProduct == SelectedProduct.Id);
            if (imageproducts != null)
            {
                foreach (var imageproduct in imageproducts)
                {
                    if (imageproduct.Source.Contains("https://firebasestorage.googleapis.com") && !ImageProducts.Any(p => (p.BMImage.UriSource != null  && p.BMImage.UriSource.ToString() == imageproduct.Source)))
                    {
                        await FireStorageAPI.Delete(imageproduct.Source);
                        await repository.Remove(imageproduct);
                    }
                }
            }
            foreach (MImageProuct imageProductSource in ImageProducts)
            {
                if (imageProductSource.Source.Contains("https://firebasestorage.googleapis.com"))
                {
                    Models.ImageProduct imageProduct = new Models.ImageProduct() { Source = imageProductSource.Source, IdProduct = SelectedProduct.Id };
                    SelectedProduct.ImageProducts.Add(imageProduct);
                    continue;
                }
                else
                {     
                    string link = await FireStorageAPI.PushFromImage(imageProductSource.BMImage, "Product", "Image", null, $"{SelectedProduct.Id}");
                    Models.ImageProduct imageProduct;
                    imageProduct = new Models.ImageProduct() { Source = link, IdProduct = SelectedProduct.Id };
                    await repository.Add(imageProduct);
                    SelectedProduct.ImageProducts.Add(imageProduct);
                }
            }
        }
    }
}
