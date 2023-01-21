using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WPFEcommerceApp.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WPFEcommerceApp
{
    internal class NotificationViewModel : BaseViewModel
    {
        public GenericDataRepository<Models.Notification> NoteRepo;



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
            Task t = Task.Run(async () => await Load());
            while (!t.IsCompleted);
        }
        private async Task Load()
        {
            MainViewModel.SetLoading(true);
            var noteList = new ObservableCollection<Models.Notification>((await NoteRepo.GetListAsync(item => item.IdReceiver == AccountStore.instance.CurrentAccount.Id,
                                                        item => item.MUser1
                                                        )).OrderByDescending(n => n.Date));

            Notifications = new ObservableCollection<NotificationBlock>(noteList.Select(item => new NotificationBlock {
                AvaImage = String.IsNullOrEmpty(item.MUser1.SourceImageAva)?Properties.Resources.DefaultShopAvaImage: item.MUser1.SourceImageAva,
                UserName = item.MUser1.Name,
                Date = item.Date.ToString(),
                NotificationContent = item.Content
            }));
            MainViewModel.SetLoading(false);
        }
    }
}
