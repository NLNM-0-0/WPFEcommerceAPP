using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class AddNewReplyBlockViewModel : BaseViewModel
    {
        public ICommand ReplyComment{get;set;}
        private Models.MUser user;
        public Models.MUser User
        {
            get => user;
            set
            {
                user = value;
                OnPropertyChanged();
            }
        }
        public string SourceImageAva
        {
            get
            {
                if (String.IsNullOrEmpty(User?.SourceImageAva))
                {
                    return Properties.Resources.DefaultShopAvaImage;
                }
                return User.SourceImageAva;
            }
        }
        private string comment;
        public string Comment
        {
            get => comment;
            set
            {
                comment = value;
                OnPropertyChanged();
            }
        }
        public AddNewReplyBlockViewModel()
        {
            ReplyComment = new RelayCommandWithNoParameter(() => { });
        }
    }
}
