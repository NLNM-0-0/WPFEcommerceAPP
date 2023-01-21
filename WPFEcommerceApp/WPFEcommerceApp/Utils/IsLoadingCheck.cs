using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp
{
    public static class IsLoadingCheck
    {
        private static int isLoading;
        public static int IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                if(isLoading > 0)
                {
                    MainViewModel.SetLoading(true);
                }
                else
                {
                    MainViewModel.SetLoading(false);
                }
            }
        }
    }
}
