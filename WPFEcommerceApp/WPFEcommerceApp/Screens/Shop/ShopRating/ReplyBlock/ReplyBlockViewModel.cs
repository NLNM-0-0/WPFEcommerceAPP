using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp
{
    public class ReplyBlockViewModel: BaseViewModel
    {
        private Models.RatingInfo ratingInfo;
        public Models.RatingInfo RatingInfo
        {
            get { return ratingInfo; }
            set
            {
                ratingInfo = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SourceImageAva));
            }
        }

        public string SourceImageAva
        {
            get
            {
                if (String.IsNullOrEmpty(ratingInfo.MUser.SourceImageAva))
                {
                    return Properties.Resources.DefaultShopAvaImage;
                }
                return ratingInfo.MUser.SourceImageAva;
            }
        }
        public ReplyBlockViewModel(Models.RatingInfo ratingInfo)
        {
            RatingInfo = ratingInfo;
        }
    }
}
