using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class ProductInfoPortraitViewModel : BaseViewModel
    {
        private readonly AccountStore _accountStore;
        public ICommand SaveProductInfoCommand { get; set; }
        public ICommand EditProductInfoCommand { get; set; }
        public ICommand ChangeSelectedImageCommand { get; set; }
        public ICommand OpenChangeImageDialogCommand { get; set; }
        public ICommand OpenProductInfoLandscapeCommand { get; set; }
        public ICommand CheckOneSizeCommand { get; set; }
        public ICommand ChangeStatusCommand { get; set; }

        private WPFEcommerceApp.Models.Product selectedProduct;
        public WPFEcommerceApp.Models.Product SelectedProduct
        {
            get
            {
                if (selectedProduct == null)
                {
                    selectedProduct = new WPFEcommerceApp.Models.Product();
                }
                return selectedProduct;
            }
            set
            {
                selectedProduct = value;
                Task task = Task.Run(async () =>
                {
                    await LoadRating();
                });
                OnPropertyChanged();
            }
        }
        private string selectedImage;
        public string SelectedImage
        {
            get
            {
                return selectedImage;
            }
            set
            {
                selectedImage = value;

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
        private IList<Models.Brand> brands;
        public IList<Models.Brand> Brands
        {
            get => brands;
            set
            {
                brands = value;
                OnPropertyChanged();
            }
        }
        private IList<Models.Category> categories;
        public IList<Models.Category> Categories
        {
            get => categories;
            set
            {
                categories = value;
                OnPropertyChanged();
            }
        }
        private double rating;
        public double Rating
        {
            get => rating;
            set
            {
                rating = value;
                OnPropertyChanged();
            }
        }
        private int ratingTimes;
        public int RatingTimes
        {
            get => ratingTimes;
            set
            {
                ratingTimes = value;
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
        public ProductInfoPortraitViewModel(AccountStore accountStore, Models.Product product)
        {
            _accountStore = accountStore;
            SelectedProduct = product;
            if(SelectedProduct.IsOneSize)
            {
                IsHadOneSize = true;
            }    
            else
            {
                IsHadOneSize = false;
            }    
            IsEditting = false;
            ImageProducts = new ObservableCollection<string>();
            SelectedImage = "";
            Task task = Task.Run(async () => { await LoadBrands(); await LoadCategories(); });
            if (SelectedProduct.ImageProducts.Count != 0)
            {
                ImageProducts.Clear();
                foreach(var item in SelectedProduct.ImageProducts)
                {
                    ImageProducts.Add(item.Source);
                }    
            }
            if (ImageProducts.Count == 0)
            {
                SelectedImage = Properties.Resources.DefaultProductImage;
            }
            else
            {
                SelectedImage = ImageProducts.First();
            }
            SaveProductInfoCommand = new RelayCommand<Models.Product>((p) => { return p != null; }, (p) =>
            {
                Models.Product productTemp = p as WPFEcommerceApp.Models.Product;
                SelectedProduct.Name = productTemp.Name;
                SelectedProduct.Price = productTemp.Price;
                SelectedProduct.ImageProducts.Clear();
                foreach (string imageProductSource in ImageProducts)
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
                SelectedProduct.Color = productTemp.Color;
                IsEditting = false;
                product = SelectedProduct;
                Task t = Task.Run(async () => { await UpdateImageProduct(); await UpdateProduct(); });
                while (!t.IsCompleted) ;
                t = Task.Run(async () => { await _accountStore.Update(_accountStore.CurrentAccount); });
                while (!t.IsCompleted) ;
            });

            EditProductInfoCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                IsEditting = true;
            });

            ChangeSelectedImageCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                Image image = p as Image;
                if(image.Source == null)
                {
                    return;
                }    
                SelectedImage = image.Source.ToString();
            });

            OpenChangeImageDialogCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                ChangeImageProductDialog changeImageProductDialog = new ChangeImageProductDialog();
                changeImageProductDialog.DataContext = new ChangeImageProductDialogViewModel(ImageProducts);
                DialogHost.Show(changeImageProductDialog, "App");
            });

            OpenProductInfoLandscapeCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                ProductInfoLandscape productInfoLandscape = new ProductInfoLandscape();
                productInfoLandscape.DataContext = new ProductInfoLandscapeViewModel(_accountStore, SelectedProduct);
                DialogHost.Show(productInfoLandscape, "App", closeProductInfoLandscape);
            });
            CheckOneSizeCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                Grid grid = p as Grid;
                foreach (var item in grid.Children)
                {
                    if (item.GetType() == typeof(ToggleButton))
                    {
                        ToggleButton button = (ToggleButton)item;
                        button.IsChecked = !IsHadOneSize;
                    }
                }
            });
            ChangeStatusCommand = new RelayCommand<object>((p) => p!=null, (p) =>
            {
                Button button = p as Button;
                if(IsEditting && button.IsEnabled)
                {
                    button.Command.Execute(button.CommandParameter);
                    IsEditting = !IsEditting;
                }    
                else if(!IsEditting)
                {
                    IsEditting = !IsEditting;
                }    
            });
        }
        private void closeProductInfoLandscape(object sender, DialogClosingEventArgs eventArgs)
        {
            ImageProducts.Clear();
            foreach(var item in SelectedProduct.ImageProducts)
            {
                ImageProducts.Add(item.Source);
            }    
            if (ImageProducts.Count > 0 && SelectedImage == Properties.Resources.DefaultProductImage)
            {
                SelectedImage = ImageProducts.First();
            }
            else if (SelectedImage != Properties.Resources.DefaultProductImage && !ImageProducts.Contains(SelectedImage))
            {
                if (ImageProducts.Count == 0)
                {
                    SelectedImage = Properties.Resources.DefaultProductImage;
                }
                else
                {
                    SelectedImage = ImageProducts.First();
                }
            }
            Task t = Task.Run(async () => await _accountStore.Update(_accountStore.CurrentAccount));
            while (!t.IsCompleted) ;
        }

        private void ImageSources_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (ImageProducts.Count > 0 && SelectedImage == Properties.Resources.DefaultProductImage)
            {
                SelectedImage = ImageProducts.First();
            }
            else if (SelectedImage != Properties.Resources.DefaultProductImage && !ImageProducts.Contains(SelectedImage))
            {
                if (ImageProducts.Count == 0)
                {
                    SelectedImage = Properties.Resources.DefaultProductImage;
                }
                else
                {
                    SelectedImage = ImageProducts.First();
                }
            }
            
        }
        private async Task LoadBrands()
        {
            var repository = new GenericDataRepository<Models.Brand>();
            Brands = await repository.GetListAsync(b=>b.Status == "NotBanned");
        }
        private async Task LoadCategories()
        {
            var repository = new GenericDataRepository<Models.Category>();
            Categories = await repository.GetListAsync(c => c.Status == "NotBanned");
        }
        private async Task LoadRating()
        {
            var repository = new GenericDataRepository<Models.Rating>();
            IList<Rating> ratingInfos = (await repository.GetListAsync(x => x.OrderInfoes.First().IdProduct == SelectedProduct.Id));
            Rating = ratingInfos.Average(p=>(p.Rating1 == null?0:p.Rating1)) ?? 0;
            RatingTimes = ratingInfos.Count();
        }
        private async Task UpdateProduct()
        {
            var repository = new GenericDataRepository<Models.Product>();
            Models.Product product = await repository.GetSingleAsync(p => p.Id == SelectedProduct.Id);
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
            await repository.Update(product);
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
