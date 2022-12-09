using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFEcommerceApp
{
    public class AddBrandDialogViewModel : BaseViewModel
    {
        private string brandName = "";
        public string BrandName
        {
            get { return brandName; }
            set
            {
                brandName = value;
                OnPropertyChanged();
            }
        }
        private string reason = "";
        public string Reason
        {
            get { return reason; }
            set
            {
                reason = value;
                OnPropertyChanged();
            }
        }
        public ICommand RequestBrandCommand { get; set; }
        public AddBrandDialogViewModel()
        {
            RequestBrandCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                //Lưu dữ liêu xuống data base

                //Tắt dialogHost
                var closeDialog = DialogHost.CloseDialogCommand;
                closeDialog.Execute(null, null);
            });
        }
    }
}
