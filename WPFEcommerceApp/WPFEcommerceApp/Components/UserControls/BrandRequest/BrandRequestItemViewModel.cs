
using System.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;


namespace WPFEcommerceApp
{
    public class BrandRequestItemViewModel:BaseViewModel
    {
        #region Public Properties

        private string _requestId;
        public string RequestId
        {
            get => _requestId;
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
            set { _name = value; OnPropertyChanged(); }
        }

        private string _id;
        public string Id
        {
            get => _id;
            set
            {
                _id = value; OnPropertyChanged();
            }
        }
        private string _sourceImageAva;
        public string SourceImageAva
        {
            get { return string.IsNullOrEmpty(_sourceImageAva)?Properties.Resources.DefaultShopAvaImage: _sourceImageAva; }
            set { _sourceImageAva = value; OnPropertyChanged(); }
        }
        public bool IsDropdown { get; set; }

        public string BrandName { get; set; }
        public string Reason { get; set; }

        #endregion

        #region Commands
        public ICommand DropdownButtonCommand { get; set; }

        #endregion

        #region Constructor
        public BrandRequestItemViewModel()
        {
            DropdownButtonCommand = new RelayCommandWithNoParameter(DropdownButton);

        }
        #endregion

        #region Command Methods

        public void DropdownButton()
        {
            IsDropdown ^= true;

        }
        #endregion


    }
}
