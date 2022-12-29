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

namespace WPFEcommerceApp
{
    public class AdsManagerViewModel : BaseViewModel
    {
        #region Commands

        private GenericDataRepository<Advertisement> adsRepo;
        public ICommand AddBackgroundImageCommand { get; set; }
        public ICommand AddAdsCommand { get; set; }
        public ICommand CancelAdsCommand { get; set; }
        public ICommand RemoveAdsCommand { get; set; }
        public ICommand ApplyAdsCommand { get; set; }
        public ICommand SelectedCommand { get; set; }
        #endregion

        #region Properties

        public ObservableCollection<Advertisement> Ads { get; set; }
        public ObservableCollection<Advertisement> InUseAds { get; set; }

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
            CurrentAds = new Advertisement();

            Task.Run(async () => await Load());

            SelectedCommand = new RelayCommand<object>(p => p != null, Selected);
            RemoveAdsCommand = new RelayCommand<object>(p => _selectedItem != null, async(p)=>await RemoveAds(p));
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
            MainViewModel.IsLoading = false;

        }

        public async Task Load()
        {
            Ads = new ObservableCollection<Advertisement>(
                await adsRepo.GetAllAsync());

            var inUse = new List<Advertisement>(
                await adsRepo.GetListAsync(ad => ad.Status == "InUse"));

            inUse.Sort(comparisonToPosition);
            InUseAds = new ObservableCollection<Advertisement>(inUse);

        }

        private int comparisonToPosition(Advertisement x, Advertisement y)
        {
            if (x == null || y == null)
                return 0;
            if (x.Position > y.Position)
                return 1;
            else if (x.Position < y.Position)
                return -1;
            else
                return 0;
        }

        #endregion

        #region Command Methods
        public async Task AddAds(object obj)
        {
            MainViewModel.IsLoading = true;

            if (string.IsNullOrEmpty(CurrentAds.Image))
                return;

            CurrentAds.Id = await GenerateID.Gen(typeof(Advertisement));

            CurrentAds.Image = await FireStorageAPI.Push(CurrentAds.Image, "Default", $"Banner_{CurrentAds.Id}");
            CurrentAds.Status = "NotUse";
            await adsRepo.Add(CurrentAds);
            await Load();
            Image = null;
            MainViewModel.IsLoading = false;

            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public void CancelAds()
        {
            Image = null;
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public async Task RemoveAds(object obj)
        {
            MainViewModel.IsLoading = true;

            if (SelectedItem == null)
                return;

            if (SelectedItem.Status == "InUse")
            {

                var view = new ConfirmDialog()
                {
                    Header = "Banner is in use",
                    Content = "Change the in use banner to another and try again.",
                };
                await DialogHost.Show(view, "adsView");
            }
            else
            {
                await adsRepo.Remove(_selectedItem);
                await Load();
            }
            MainViewModel.IsLoading = false;

        }

        public async Task ApplyAds(object obj)
        {
            var ad = obj as Advertisement;
            if (ad == null)
                return;

            if (ad.Status == "InUse")
            {
                var view = new ConfirmDialog()
                {
                    Header = "Banner is currently in use",
                    Content = "Change the in use banner to another and try again.",
                };
                await DialogHost.Show(view, "adsView");
                return;
            }

            MainViewModel.IsLoading = true;

            for (int i = 1; i < 4; i++)
            {
                if (CurrentPos == i.ToString())
                {
                    var lastAd = await adsRepo.GetSingleAsync(item => item.Position == i && item.Status == "InUse");
                    if (lastAd != null)
                    {
                        lastAd.Status = "NotUse";
                        await adsRepo.Update(lastAd);
                    }

                    ad.Status = "InUse";
                    ad.Position = i;
                    await adsRepo.Update(ad);
                    break;
                }
            }

            await Load();
            MainViewModel.IsLoading = false;

        }

        public void Selected(object obj)
        {
            var ads = obj as Advertisement;
            SelectedItem = ads;
        }
        #endregion
    }
}
