
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DataAccessLayer;
using System.Xml.Linq;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class AdsManagerViewModel:BaseViewModel
    {
        #region Commands

        private GenericDataRepository<Advertisement> adsRepo;
        public ICommand AddBackgroundImageCommand { get; set; }
        public ICommand AddAdsCommand { get; set; }
        public ICommand CancelAdsCommand { get; set; }
        public ICommand RemoveAdsCommand { get; set; }
        public ICommand RadioCommand { get; set; }
        #endregion

        #region Properties

        public ObservableCollection<Advertisement> Ads { get; set; }

        private ImageSource _sourceImage;
        public ImageSource SourceImage
        {
            get { return _sourceImage; }
            set { _sourceImage = value; OnPropertyChanged(); }
        }

        private Advertisement _selectedItem;

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
        #endregion

        #region Construstor
        public AdsManagerViewModel()
        {
            adsRepo= new GenericDataRepository<Advertisement>();
            CurrentAds = new Advertisement();
            Load();

            RadioCommand = new RelayCommand<object>(p => p != null, Radio);
            RemoveAdsCommand = new RelayCommand<object>(p=>_selectedItem!=null, RemoveAds);
            CancelAdsCommand = new RelayCommandWithNoParameter(CancelAds);
            AddAdsCommand = new RelayCommandWithNoParameter(AddAds);
            AddBackgroundImageCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png; *.webp";
                op.ShowDialog();
                if (op.FileName != "")
                {
                    Image = op.FileName;
                    CurrentAds.Image=Image;
                }
            });
        }

        public async void Load()
        {
            Ads = new ObservableCollection<Advertisement>(
                await adsRepo.GetAllAsync());
        }

        #endregion

        #region Command Methods

        public void Radio(object obj)
        {
            _selectedItem=obj as Advertisement;
            
        }
        public async void AddAds()
        {
            if (string.IsNullOrEmpty(CurrentAds.Title) || string.IsNullOrEmpty(CurrentAds.SubTitle)
                || string.IsNullOrEmpty(CurrentAds.Image))
                return;

            CurrentAds.Id = await GenerateID.Gen(typeof(Advertisement));
            await adsRepo.Add(CurrentAds);
            Load();
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public void CancelAds()
        {
            
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public async void RemoveAds(object obj)
        {
            //To do: check if the ads is used
            if (_selectedItem == null)
                return;
            
            await adsRepo.Remove(_selectedItem);
            Load();
        }
        #endregion
    }
}
