using DataAccessLayer;
using MaterialDesignThemes.Wpf.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Media3D;
using WPFEcommerceApp.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WPFEcommerceApp
{
    internal class HomeViewModel:BaseViewModel
    {
           
        public GenericDataRepository<Models.Product> ProductRepository { get; set; }
        private ObservableCollection<Models.Product> products;
        public ICommand SearchCommand { get; set; }
        public ObservableCollection<Models.Product> Products
        {
            get { return products; }
            set { products = value; 
            OnPropertyChanged();}
            
        }
        private ObservableCollection<ProductBlockViewModel> productViewModels;
        public ObservableCollection<ProductBlockViewModel> ProductViewModels
        {
            get { return productViewModels; }
            set
            {
                productViewModels = value;
                OnPropertyChanged();
            }

        }
        private bool newProduct;
        public bool NewProdcut
        {
            get => newProduct;
            set
            {
                newProduct = value;
                if (value)
                {
                    
                }
            }
        }
       
        private bool isCheckCategory;
        public bool IsCheckCategory
        {
            get => isCheckCategory;
            set
            {
                isCheckCategory = value;
                if (value)
                {
                    Task task = Task.Run(async () => await LoadCategory());
                    while (!task.IsCompleted) ;
                    LoadProductBLockViewModel();
                }
            }
        }

        private bool isCheckBrand;
        public bool IsCheckBrand
        {
            get => isCheckBrand;
            set
            {
                isCheckBrand = value;
                if (value)
                {
                    Task task = Task.Run(async () => await LoadBrand());
                    while (!task.IsCompleted) ;
                    LoadProductBLockViewModel();
                }
            }
        }
        private bool sortPrice0To200k;
        public bool SortPrice0To200k
        {
            get => sortPrice0To200k;
            set
            {
                if(value)
                {
                    MinPrice = 0;
                    MaxPrice = 200000;
                    SearchRadioButton();
                }
                sortPrice0To200k = value;
                OnPropertyChanged();
            }
        }
        private bool sortPrice200kTo500k;
        public bool SortPrice200kTo500k
        {
            get => sortPrice200kTo500k;
            set
            {
                if (value)
                {
                    MinPrice = 200000;
                    MaxPrice = 500000;
                    SearchRadioButton();
                }
                sortPrice200kTo500k = value;
                OnPropertyChanged();
            }
        }
        private bool sortPrice500kTo1000k;
        public bool SortPrice500kTo1000k
        {
            get => sortPrice500kTo1000k;
            set
            {
                if (value)
                {
                    MinPrice = 500000;
                    MaxPrice = 1000000;
                    SearchRadioButton();
                }
                sortPrice500kTo1000k = value;
                OnPropertyChanged();
            }
        }
        private bool sortPriceP1000k;
        public bool SortPriceP1000k
        {
            get => sortPriceP1000k;
            set
            {
                if (value)
                {
                    MinPrice = 1000000;
                    MaxPrice = long.MaxValue;
                    SearchRadioButton();
                }
                sortPriceP1000k = value;
                OnPropertyChanged();
            }
        }
        private bool isBigDiscount;
        public bool IsBigDiscount
        {
            get => isBigDiscount;
            set
            {
                isBigDiscount = value;
                if (value)
                {
                    Task task = Task.Run(async () => await LoadBigDiscount());
                    while (!task.IsCompleted) ;
                    LoadProductBLockViewModel();
                }
            }
        }
        private bool isNew;
        public bool IsNew
        {
            get => isNew;
            set
            {
                isNew = value;
                if (value)
                {
                    Task task = Task.Run(async () => await LoadNew());
                    while (!task.IsCompleted) ;
                    LoadProductBLockViewModel();
                }
            }
        }
        private bool isBestSeller;
        public bool IsBestSeller
        {
            get => isBestSeller;
            set
            {
                isBestSeller = value;
                if (value)
                {
                    Task task = Task.Run(async () => await LoadBestSeller());
                    while (!task.IsCompleted) ;
                    LoadProductBLockViewModel();
                }
            }
        }
        private bool isNoFilter;
        public bool IsNoFilter
        {
            get => isNoFilter;
            set
            {
                isNoFilter = value;
                if (value)
                {
                    Task task = Task.Run(async () => await Load());
                    while (!task.IsCompleted) ;
                    LoadProductBLockViewModel();
                }
            }
        }
        private bool isHadOneSize;
        public bool IsHadOneSize
        {
            get => isHadOneSize;
            set
            {
                if (value)
                {
                    CheckSize[0] = true;
                    CheckSize[1] = CheckSize[2] = CheckSize[3] = CheckSize[4] = CheckSize[5] =false;
                    SearchRadioButton();
                }
                isHadOneSize = value;
                OnPropertyChanged();
            }
        }
        private bool isHadSizeS;
        public bool IsHadSizeS
        {
            get => isHadSizeS;
            set
            {
                if (value)
                {
                    CheckSize[1] = true;
                    CheckSize[0] = CheckSize[2] = CheckSize[3] = CheckSize[4] = CheckSize[5] = false;
                    SearchRadioButton();
                }
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
                if (value)
                {
                    CheckSize[2] = true;
                    CheckSize[0] = CheckSize[1] = CheckSize[3] = CheckSize[4] = CheckSize[5] = false;
                    SearchRadioButton();
                }
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
                if (value)
                {
                    CheckSize[3] = true;
                    CheckSize[0] = CheckSize[2] = CheckSize[1] = CheckSize[4] = CheckSize[5] = false;
                    SearchRadioButton();
                }
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
                if (value)
                {
                    CheckSize[4] = true;
                    CheckSize[0] = CheckSize[2] = CheckSize[3] = CheckSize[1] = CheckSize[5] = false;
                    SearchRadioButton();
                }
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
                if (value)
                {
                    CheckSize[5] = true;
                    CheckSize[0] = CheckSize[2] = CheckSize[3] = CheckSize[4] = CheckSize[1] = false;
                    SearchRadioButton();
                }
                isHadSizeXXL = value;
                OnPropertyChanged();
            }
        }
        private long minPrice = long.MinValue;
        public long MinPrice
        {
            get => minPrice;
            set
            {
                minPrice = value;
                OnPropertyChanged();
            }
        }
        private long maxPrice = long.MaxValue;
        public long MaxPrice
        {
            get => maxPrice;
            set
            {
                maxPrice = value;
                OnPropertyChanged();
            }
        }
        public bool[] CheckSize = { false, false, false, false, false, false };

        public HomeViewModel()
        {
            ProductViewModels = new ObservableCollection<ProductBlockViewModel>();
            ProductRepository = new GenericDataRepository<Models.Product>();
            Task task = Task.Run(async () => { await Load(); await LoadCategoryCheckBox(); await LoadBrandCheckBox(); });
            while (!task.IsCompleted) ;
            LoadProductBLockViewModel();
            SearchCommand = new RelayCommandWithNoParameter(async()=>
            {
                await Search();
            });
        }
        private async void SearchRadioButton()
        {
            await Search();
        }
        private async Task Search()
        {
            List<string> listCategoryId= new List<string>();
            List<string> allCategoryId = new List<string>();
            foreach (CategoryCheckBoxViewModel categoryCheckBoxViewModel in CategoryCheckBoxViewModels)
            {
                if (categoryCheckBoxViewModel.IsChecked)
                {
                    listCategoryId.Add(categoryCheckBoxViewModel.Category.Id);
                }
                allCategoryId.Add(categoryCheckBoxViewModel.Category.Id);
            }
            if(listCategoryId.Count == 0)
            {
                listCategoryId = allCategoryId;
            }
            List<string> listBrandId = new List<string>();
            List<string> allBrandId = new List<string>();
            foreach (BrandCheckViewModel brandCheckViewModel in BrandCheckViewModels)
            {
                if (brandCheckViewModel.IsChecked)
                {
                    listBrandId.Add(brandCheckViewModel.Brand.Id);
                }
                allBrandId.Add(brandCheckViewModel.Brand.Id);
            }
            if (listBrandId.Count == 0)
            {
                listBrandId = allBrandId;
            }
            Products = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p => (p.Status == "NotBanned" &&
                                                                                                            (p.Price * (100 - p.Sale) / 100 <= MaxPrice && p.Price * (100 - p.Sale) / 100 >= MinPrice)&&
                                                                                                            (listCategoryId.Contains(p.IdCategory))&&(listBrandId.Contains(p.IdBrand))&&((p.IsOneSize == CheckSize[0] == true)||
                                                                                                            (p.IsHadSizeS == CheckSize[1] == true)||(p.IsHadSizeM == CheckSize[2]==true)||(p.IsHadSizeL == CheckSize[3] == true)||
                                                                                                            (p.IsHadSizeXL == CheckSize[4] == true)|| (p.IsHadSizeXXL == CheckSize[5] == true))),
                                                                                                        p => p.Category,
                                                                                                        p => p.Brand,
                                                                                                        p => p.ImageProducts));
            LoadProductBLockViewModel();
        }
        private void LoadProductBLockViewModel()
        {
            ProductViewModels.Clear();
            foreach (Models.Product product in Products)
            {
                ProductViewModels.Add(new ProductBlockViewModel(product));
            }    
        }
        private async Task Load()
        {
            Products = new ObservableCollection<Models.Product>( await ProductRepository.GetListAsync(  p=>p.Status == "NotBanned",
                                                                                                        p=>p.Category,
                                                                                                        p=>p.Brand,
                                                                                                        p=>p.ImageProducts));
            /*LoadProductBLockViewModel();
            OnPropertyChanged(nameof(ProductViewModels));*/
        }
        
        private async Task LoadBigDiscount()
        {
            Products = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p => p.Sale >= 20,
                                                                                                        p => p.Category,
                                                                                                        p => p.Brand,
                                                                                                        p => p.ImageProducts));
           /* LoadProductBLockViewModel();
            OnPropertyChanged(nameof(ProductViewModels));*/
        }
        private async Task LoadNew()
        {
            Products = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p => (DateTime.Today - p.DateOfSale) < new TimeSpan(7,0,0,0),
                                                                                                        p => p.Category,
                                                                                                        p => p.Brand,
                                                                                                        p => p.ImageProducts));
            /*LoadProductBLockViewModel();
            OnPropertyChanged(nameof(ProductViewModels));*/
        }
        private async Task LoadBestSeller()
        {
            Products = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p => p.Sold >= 100,
                                                                                                        p => p.Category,
                                                                                                        p => p.Brand,
                                                                                                        p => p.ImageProducts));
           /* LoadProductBLockViewModel();
            OnPropertyChanged(nameof(ProductViewModels));*/
        }
        

        private async Task LoadCategory()
        {
            List<string> hellos = new List<string>();
            foreach (CategoryCheckBoxViewModel categoryCheckBoxViewModel in CategoryCheckBoxViewModels)
            {
                if (categoryCheckBoxViewModel.IsChecked)
                {
                    hellos.Add(categoryCheckBoxViewModel.Category.Name);
                }
            }
            Products = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p => hellos.Contains(p.Category.Name),
                                                                                                        p => p.Category,
                                                                                                        p => p.Brand,
                                                                                                        p => p.ImageProducts)) ;
            /* LoadProductBLockViewModel();
             OnPropertyChanged(nameof(ProductViewModels));*/
        }

        private async Task LoadBrand()
        {
            List<string> hellos = new List<string>();
            foreach (BrandCheckViewModel brandCheckViewModel in BrandCheckViewModels)
            {
                if (brandCheckViewModel.IsChecked)
                {
                    hellos.Add(brandCheckViewModel.Brand.Name);
                }
            }
            Products = new ObservableCollection<Models.Product>(await ProductRepository.GetListAsync(p => hellos.Contains(p.Brand.Name),
                                                                                                        p => p.Category,
                                                                                                        p => p.Brand,
                                                                                                        p => p.ImageProducts));
            /* LoadProductBLockViewModel();
             OnPropertyChanged(nameof(ProductViewModels));*/
        }
        private ObservableCollection<CategoryCheckBoxViewModel> categoryCheckBoxViewModels = new ObservableCollection<CategoryCheckBoxViewModel>();
        public ObservableCollection<CategoryCheckBoxViewModel> CategoryCheckBoxViewModels
        {
            get => categoryCheckBoxViewModels;
            set
            {
                categoryCheckBoxViewModels = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<BrandCheckViewModel> brandCheckViewModels = new ObservableCollection<BrandCheckViewModel>();
        public ObservableCollection<BrandCheckViewModel> BrandCheckViewModels
        {
            get => brandCheckViewModels;
            set
            {
                brandCheckViewModels = value;
                OnPropertyChanged();
            }
        }
        private async Task LoadCategoryCheckBox()
        {
            GenericDataRepository<Models.Category> genericDataRepository = new GenericDataRepository<Models.Category>();
            ObservableCollection<Models.Category> categories = new ObservableCollection<Models.Category>(await genericDataRepository.GetListAsync(p => p.Status != "Banned"));
            foreach(Models.Category category in categories)
            {
                CategoryCheckBoxViewModel categoryCheckBoxViewModel = new CategoryCheckBoxViewModel();
                categoryCheckBoxViewModel.Category = category;
                categoryCheckBoxViewModel.IsChecked = false;
                CategoryCheckBoxViewModels.Add(categoryCheckBoxViewModel);
            }

            
        }
        private async Task LoadBrandCheckBox()
        {
            GenericDataRepository<Models.Brand> genericDataRepository = new GenericDataRepository<Models.Brand>();
            ObservableCollection<Models.Brand> brands = new ObservableCollection<Models.Brand>(await genericDataRepository.GetListAsync(p => p.Status != "Banned"));
            foreach (Models.Brand brand in brands)
            {
                BrandCheckViewModel brandCheckViewModel = new BrandCheckViewModel();
                brandCheckViewModel.Brand = brand;
                brandCheckViewModel.IsChecked = false;
                BrandCheckViewModels.Add(brandCheckViewModel);
            }


        }
    }
}
