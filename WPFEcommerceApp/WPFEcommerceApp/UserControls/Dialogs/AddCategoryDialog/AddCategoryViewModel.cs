using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFEcommerceApp
{
    public class AddCategoryDialogViewModel : BaseViewModel
    {
        private string categoryName = "";
        public string CategoryName
        {
            get { return categoryName; }
            set
            {
                categoryName = value;
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
        public ICommand RequestCategoryCommand { get; set; }
        public AddCategoryDialogViewModel()
        {
            RequestCategoryCommand = new RelayCommand<object>((p) => { return p != null; }, (p) =>
            {
                //Lưu dữ liêu xuống data base

                //Tắt dialogHost
                var closeDialog = DialogHost.CloseDialogCommand;
                closeDialog.Execute(null, null);
            });
        }
    }
}
