using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    internal class NotificationViewModel : BaseViewModel
    {
        public GenericDataRepository<Models.Notification> NoteRepo;
        public string ID = "1008";



        private ObservableCollection<NotificationBlock> notifications;
        public ObservableCollection<NotificationBlock> Notifications
        {
            get { return notifications; }
            set
            {
                notifications = value;
                OnPropertyChanged();
            }
        }
        public NotificationViewModel()
        {
            NoteRepo = new GenericDataRepository<Models.Notification>();
            Load();
        }
        private async void Load()
        {

            var noteList =new ObservableCollection<Models.Notification>( await NoteRepo.GetListAsync(item => item.IdReceiver == ID,
                                                        item=>item.MUser
                                                        ));

            Notifications = new ObservableCollection<NotificationBlock>(noteList.Select(item => new NotificationBlock
            {
                AvaImage = new BitmapImage(new Uri(item.MUser.SourceImageAva)),
                UserName = item.MUser.Name,
                Date = item.Date.ToString(),
                NotificationContent = item.Content
            })) ;
        }
    }
}
