using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WPFEcommerceApp
{
    public class NotificationBlock
    {
        private string avaImage;
        public string AvaImage
        {
            get => avaImage;
            set
            {
                avaImage = value;
            }
        }
        private string userName;
        public string UserName
        {
            get => userName;
            set
            {
                userName = value;
            }
        }
        private string date;
        public string Date
        {
            get => date;
            set
            {
                date = value;
            }
        }
        private string notificationContent;
        public string NotificationContent
        {
            get => notificationContent;
            set
            {
                notificationContent = value;
            }
        }
    }
}
