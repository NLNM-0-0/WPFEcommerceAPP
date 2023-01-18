
using System.Windows.Input;
using System.Windows.Media;

namespace WPFEcommerceApp
{
    public class ShopRequestItemViewModel:BaseViewModel
    {
        #region Public Properties
        private string _requestId;
        public string RequestId
        {
            get=>_requestId;
            set
            {
                _requestId = value;
                OnPropertyChanged();
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _address;
        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        private string _sourceImageAva;
        public string SourceImageAva
        {
            get { return string.IsNullOrEmpty(_sourceImageAva) ? Properties.Resources.DefaultShopAvaImage : _sourceImageAva; ; }
            set { _sourceImageAva = value; OnPropertyChanged(); }
        }

        private string _id;
        public string Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }
        #endregion

        #region Command
        public static ICommand RemoveRequestCommand { get; set; }

        public static ICommand AddRequestCommand { get; set; }
        #endregion
        public ShopRequestItemViewModel()
        {

        }
    }
}
