using System.Windows.Media;
using System.Windows.Media.Imaging;
using DataAccessLayer;
using System.Xml.Linq;
using WPFEcommerceApp.Models;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Forms;
using System.Threading.Tasks;
using System;
using System.Net.Security;
using System.Windows;

namespace WPFEcommerceApp
{
    public class AdsManagerViewModel : BaseViewModel
    {
        #region Commands

        private GenericDataRepository<Advertisement> adsRepo;
        private GenericDataRepository<AdInUse> adInUseRepo;
        public ICommand CancelAdsCommand { get; set; }
        public ICommand RemoveAdsCommand { get; set; }
        public ICommand RemoveInUseAdsCommand { get; set; }
        public ICommand ApplyAdsCommand { get; set; }
        public ICommand SelectedCommand { get; set; }
        public ICommand OpenAdsDialogCommand { get; set; }
        #endregion

        #region Properties

        public ObservableCollection<Advertisement> Ads { get; set; }
        public ObservableCollection<AdInUse> InUseAds { get; set; }

        private ImageSource _sourceImage;
        public ImageSource SourceImage
        {
            get { return _sourceImage; }
            set { _sourceImage = value; OnPropertyChanged(); }
        }

        private Advertisement _selectedItem;
        public Advertisement SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        private AdInUse _inUseSelected;
        public AdInUse InUseSelected
        {
            get => _inUseSelected;
            set
            {
                _inUseSelected = value;
                OnPropertyChanged();
            }
        }

        private string _image;
        public string Image
        {
            get => _image;
            set { _image = value; OnPropertyChanged(); }
        }

        public string CurrentPos { get; set; }
        public List<string> ComboBoxSource { get; set; } = new List<string> { "1", "2", "3" };

        private CroppedBitmap _imageAds;
        public CroppedBitmap ImageAds
        {
            get => _imageAds;
            set
            {
                _imageAds = value;
                OnPropertyChanged();
            }
        }

        public string SourceImageAds { get; set; }
        #endregion

        #region Construstor
        public AdsManagerViewModel()
        {
            MainViewModel.SetLoading(true);
            adsRepo = new GenericDataRepository<Advertisement>();
            adInUseRepo = new GenericDataRepository<AdInUse>();
            SourceImageAds=string.Empty;

            Task.Run(async () =>
            {
                MainViewModel.SetLoading(true);

                await Load();
            }).ContinueWith((first) =>
            {
                SelectedCommand = new RelayCommand<object>(p => p != null, Selected);
                RemoveAdsCommand = new RelayCommand<object>(p => _selectedItem != null, async (p) => await RemoveAds(p));
                RemoveInUseAdsCommand = new RelayCommand<object>(p => _inUseSelected != null && _inUseSelected.Id!=null, async (p) => await RemoveInUseAds(p));
                ApplyAdsCommand = new RelayCommand<object>(p => p != null && CurrentPos != null, async (p) => await ApplyAds(p));
                CancelAdsCommand = new RelayCommandWithNoParameter(CancelAds);
                OpenAdsDialogCommand = new RelayCommandWithNoParameter(async () => await OpenAdsDialog());

                MainViewModel.SetLoading(false);
            });

        }

        public async Task Load()
        {
            Ads = new ObservableCollection<Advertisement>(
                await adsRepo.GetAllAsync(item=>item.AdInUses));

            var inUseAds = new ObservableCollection<AdInUse>
            {
                await adInUseRepo.GetSingleAsync(item=>item.Position==1, item=>item.Advertisement),
                await adInUseRepo.GetSingleAsync(item=>item.Position==2, item=>item.Advertisement),
                await adInUseRepo.GetSingleAsync(item=>item.Position==3, item=>item.Advertisement),

            };

            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                Ads = new ObservableCollection<Advertisement>(Ads);
                InUseAds = new ObservableCollection<AdInUse>();
                foreach(var item in inUseAds)
                {
                    if (item == null)
                    {
                        InUseAds.Add(new AdInUse { Advertisement = new Advertisement { Image = Properties.Resources.DefaultShopBackgroundImage } }); 
                    }
                    else
                        InUseAds.Add(item);
                }

            }));
        }


        #endregion

        #region Command Methods

        public void CancelAds()
        {
            Image = null;
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public async Task RemoveAds(object obj)
        {
            if (SelectedItem == null)
                return;

            if (SelectedItem.AdInUses.Count != 0)
            {
                var view = new ConfirmDialog()
                {
                    CM = new RelayCommandWithNoParameter(async () =>
                    {
                        MainViewModel.SetLoading(true);
                        await adInUseRepo.Remove(SelectedItem.AdInUses.ToArray());
                        await RemoveAdsDB();
                        CommandManager.InvalidateRequerySuggested();
                    }),
                    Header = "Banner is in use",
                    Content = "Are you sure to remove it?",
                };
                await DialogHost.Show(view, "adsView");
            }
            else
                await RemoveAdsDB();


        }

        public async Task RemoveInUseAds(object obj)
        {
            if (InUseSelected == null)
                return;

            var view = new ConfirmDialog()
            {
                CM = new RelayCommandWithNoParameter(async () =>
                {
                    MainViewModel.SetLoading(true);
                    await RemoveInUseAdsDB();
                    CommandManager.InvalidateRequerySuggested();
                }),
                Header = "Banner is in use",
                Content = "Are you sure to remove it?",
            };
            await DialogHost.Show(view, "adsView");

        }

        public async Task RemoveAdsDB()
        {
            MainViewModel.SetLoading(true);

            await adsRepo.Remove(_selectedItem);
            await FireStorageAPI.Delete(_selectedItem.Image);
            await Load();
            MainViewModel.SetLoading(false);

        }

        public async Task RemoveInUseAdsDB()
        {
            MainViewModel.SetLoading(true);

            var current = await adInUseRepo.GetSingleAsync(item => item.Id == _inUseSelected.Id);

            if(current!=null)
                await adInUseRepo.Remove(current);

            await Load();
            MainViewModel.SetLoading(false);

        }

        public async Task ApplyAds(object obj)
        {
            var ad = obj as Advertisement;
            if (ad == null)
                return;

            foreach(var item in InUseAds)
            {
                if(item!=null)
                {
                    if(item.Advertisement!=null&&item.Advertisement.Id==ad.Id)
                    {
                        string content;
                        if (item.Position.ToString() == CurrentPos)
                            content = "The banner has been place at the position chosen!";

                        else
                            content = "The banner has been placed at another position, cannot use it now!";
                        var existed = new ConfirmDialog()
                        {
                            Header = "Already set",
                            Content = content
                        };
                        await DialogHost.Show(existed, "adsView");
                        return;
                    }
                }
            }
            foreach (var item in InUseAds)
            {
                if(item!= null)
                {
                    if (item.Position.ToString() == CurrentPos&&item.Advertisement!=null&&item.Advertisement.Id!=null)
                    {
                        var view = new ConfirmDialog()
                        {
                            Header = "Replace",
                            Content = "The position chosen has already had a banner, are you sure you want to replace it?",
                            CM = new RelayCommandWithNoParameter(async () =>
                            {
                                MainViewModel.SetLoading(true);
                                await adInUseRepo.Remove(item);
                                await SetAds(ad);
                            })
                        };
                        await DialogHost.Show(view, "adsView");
                        MainViewModel.SetLoading(false);

                        return;
                    }
                }
                
            }
            MainViewModel.SetLoading(true);

            await SetAds(ad);
            MainViewModel.SetLoading(false);

        }

        private async Task SetAds(Advertisement ad)
        {
            var adInUse = new AdInUse
            {
                Position = int.Parse(CurrentPos),
                Id = ad.Id,
            };

            await adInUseRepo.Add(adInUse);
            await Load();
            MainViewModel.SetLoading(false);


        }
        public void Selected(object obj)
        {
            var ads = obj as Advertisement;

            if (ads.Id == null)
                return;

            SelectedItem = ads;
            foreach(var item in InUseAds)
            {
                if (item.Id == null)
                    continue;
                if(ads.Id==item.Id)
                {
                    InUseSelected = item;
                }
            }
        }

        public async Task OpenAdsDialog()
        {
            var dialog = new AddAdsDialog();
            ImageAds = new CroppedBitmap(new BitmapImage(new Uri(Properties.Resources.DefaultShopBackgroundImage)), new Int32Rect(0, 0, 0, 0));
            dialog.DataContext = new AdsDialogViewModel(ImageAds);
            await DialogHost.Show(dialog, "Main", null, null, SaveAds);
        }

        private async void SaveAds(object sender, DialogClosedEventArgs eventArgs)
        {
            MainViewModel.SetLoading(true);
            if (eventArgs.Parameter != null && eventArgs.Parameter.GetType() == typeof(CroppedBitmap))
            {
                ImageAds = (eventArgs.Parameter as CroppedBitmap);
                var ads = new Advertisement();
                ads.Id = await GenerateID.Gen(typeof(Advertisement));
                string link = await FireStorageAPI.PushFromImage((BitmapSource)ImageAds, "Default", $"Banner_{ads.Id}");

                ads.Image = link;
                await adsRepo.Add(ads);
                await Load();
            }

            MainViewModel.SetLoading(false);
        }
        #endregion
    }
}
