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

namespace WPFEcommerceApp
{
    public class AdsManagerViewModel : BaseViewModel
    {
        #region Commands

        private GenericDataRepository<Advertisement> adsRepo;
        private GenericDataRepository<AdInUse> adInUseRepo;
        public ICommand AddBackgroundImageCommand { get; set; }
        public ICommand AddAdsCommand { get; set; }
        public ICommand CancelAdsCommand { get; set; }
        public ICommand RemoveAdsCommand { get; set; }
        public ICommand RemoveInUseAdsCommand { get; set; }
        public ICommand ApplyAdsCommand { get; set; }
        public ICommand SelectedCommand { get; set; }
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

        private Advertisement _currentAds;
        public Advertisement CurrentAds
        {
            get => _currentAds;
            set { _currentAds = value; OnPropertyChanged(); }
        }
        private string _image;
        public string Image
        {
            get => _image;
            set { _image = value; OnPropertyChanged(); }
        }

        public string CurrentPos { get; set; }
        public List<string> ComboBoxSource { get; set; } = new List<string> { "1", "2", "3" };

        #endregion

        #region Construstor
        public AdsManagerViewModel()
        {
            MainViewModel.IsLoading = true;
            adsRepo = new GenericDataRepository<Advertisement>();
            adInUseRepo = new GenericDataRepository<AdInUse>();
            CurrentAds = new Advertisement();

            SelectedCommand = new RelayCommand<object>(p => p != null, Selected);
            RemoveAdsCommand = new RelayCommand<object>(p => _selectedItem != null, async(p)=>await RemoveAds(p));
            RemoveInUseAdsCommand = new RelayCommand<object>(p => _inUseSelected != null, async (p) => await RemoveInUseAds(p));
            ApplyAdsCommand = new RelayCommand<object>(p => p != null && CurrentPos != null, async(p)=>await ApplyAds(p));
            CancelAdsCommand = new RelayCommandWithNoParameter(CancelAds);
            AddAdsCommand = new RelayCommand<object>(p=>p!=null, async(p)=>await AddAds(p));
            AddBackgroundImageCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png; *.webp";
                op.ShowDialog();
                if (op.FileName != "")
                {
                    Image = op.FileName;
                    CurrentAds.Image = Image;

                }
            });

            Task.Run(async () =>
            {
                MainViewModel.IsLoading = true;
                await Load();
            }).ContinueWith((first) =>
            {
                MainViewModel.IsLoading = false;

            });

        }

        public async Task Load()
        {
            Ads = new ObservableCollection<Advertisement>(
                await adsRepo.GetAllAsync(item=>item.AdInUses));

            InUseAds = new ObservableCollection<AdInUse>
            {
                await adInUseRepo.GetSingleAsync(item=>item.Position==1, item=>item.Advertisement),
                await adInUseRepo.GetSingleAsync(item=>item.Position==2, item=>item.Advertisement),
                await adInUseRepo.GetSingleAsync(item=>item.Position==3, item=>item.Advertisement),

            };
            var temp = InUseAds[0];
        }


        #endregion

        #region Command Methods
        public async Task AddAds(object obj)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);

            MainViewModel.IsLoading = true;

            if (string.IsNullOrEmpty(CurrentAds.Image))
                return;

            CurrentAds.Id = await GenerateID.Gen(typeof(Advertisement));

            CurrentAds.Image = await FireStorageAPI.Push(CurrentAds.Image, "Default", $"Banner_{CurrentAds.Id}");
            await adsRepo.Add(CurrentAds);
            await Load();
            Image = null;
            MainViewModel.IsLoading = false;

        }

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
                        MainViewModel.IsLoading = true;
                        await adInUseRepo.Remove(SelectedItem.AdInUses.ToArray());
                        await RemoveAdsDB();
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
                    MainViewModel.IsLoading = true;
                    await adInUseRepo.Remove(SelectedItem.AdInUses.ToArray());
                    await RemoveInUseAdsDB();
                }),
                Header = "Banner is in use",
                Content = "Are you sure to remove it?",
            };
            await DialogHost.Show(view, "adsView");

        }

        public async Task RemoveAdsDB()
        {
            MainViewModel.IsLoading = true;

            await adsRepo.Remove(_selectedItem);
            await FireStorageAPI.Delete(_selectedItem.Image);
            await Load();
            MainViewModel.IsLoading = false;

        }

        public async Task RemoveInUseAdsDB()
        {
            MainViewModel.IsLoading = true;

            var current = await adInUseRepo.GetSingleAsync(item => item.Id == _inUseSelected.Id);

            if(current!=null)
                await adInUseRepo.Remove(current);

            await Load();
            MainViewModel.IsLoading = false;

        }

        public async Task ApplyAds(object obj)
        {
            var ad = obj as Advertisement;
            if (ad == null)
                return;

            foreach (var item in InUseAds)
            {
                if(item!= null)
                {
                    if (item.Position.ToString() == CurrentPos)
                    {
                        if(item.Advertisement.Id==ad.Id)
                        {
                            var existed = new ConfirmDialog()
                            {
                                Header = "Already set",
                                Content = "The banner has been placed at the position chosen!"
                            };
                            await DialogHost.Show(existed, "adsView");
                            return;
                        }
                        var view = new ConfirmDialog()
                        {
                            Header = "Replace",
                            Content = "The position chosen has already had a banner, are you sure you want to replace it?",
                            CM = new RelayCommandWithNoParameter(async () =>
                            {
                                MainViewModel.IsLoading = true;
                                await adInUseRepo.Remove(item);
                                await SetAds(ad);
                            })
                        };
                        await DialogHost.Show(view, "adsView");
                        MainViewModel.IsLoading = false;

                        return;
                    }
                }
                
            }
            MainViewModel.IsLoading = true;

            await SetAds(ad);
            MainViewModel.IsLoading = false;

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
            MainViewModel.IsLoading = false;


        }
        public void Selected(object obj)
        {
            var ads = obj as Advertisement;
            
            SelectedItem = ads;
            foreach(var item in InUseAds)
            {
                if (item != null && item.Id == SelectedItem.Id)
                    InUseSelected = item;
            }
        }
        #endregion
    }
}
