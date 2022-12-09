using System.Windows.Input;
using System.Windows.Media;

namespace WPFEcommerceApp
{
    public class CategoryRequestItemViewModel : BaseViewModel
    {
        #region Public Properties

        private int _requestId;
        public int RequestId
        {
            get => _requestId;
            set
            {
                _requestId = value;
                OnPropertyChanged();
            }
        }

        private string _reason;
        public string Reason
        {
            get { return _reason; }
            set { _reason = value; OnPropertyChanged(); }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                _id = value; OnPropertyChanged();
            }
        }
        private ImageSource _sourceImageAva;
        public ImageSource SourceImageAva
        {
            get { return _sourceImageAva; }
            set { _sourceImageAva = value; OnPropertyChanged(); }
        }

        public bool IsDropdown { get; set; }

        public string CategoryName { get; set; }

        #endregion

        #region Commands
        public ICommand DropdownButtonCommand { get; set; }

        #endregion

        #region Constructor
        public CategoryRequestItemViewModel()
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
