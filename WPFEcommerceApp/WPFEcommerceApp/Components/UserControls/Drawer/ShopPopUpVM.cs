using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFEcommerceApp {
    public class ShopPopUpVM : BaseViewModel {
        private bool isFirstTime;

        public bool IsFirstTime {
            get { return isFirstTime; }
            set { isFirstTime = value; OnPropertyChanged(); }
        }

        public ObservableCollection<bool> SelectedIndex { get; set; }

        public ICommand OnChecked { get; set; }
        static void tempFunc(ObservableCollection<bool> p, int id, ref bool isFirstTime) {
                isFirstTime = false;
                for(int i = 0; i<p.Count; i++) {
                    if(id == i) p[i] = true;
                    else p[i] = false;
                }
        }
        public ShopPopUpVM(DrawerVM drawerVM) {

            isFirstTime = drawerVM.SelectedIndex == 4;

            SelectedIndex = new ObservableCollection<bool>() { false, false, false, false, false };

            OnChecked = new RelayCommand<object>(p => true, p => {
                drawerVM.CanReload = false;
                drawerVM.SelectedIndex = 4;
                var temp = Convert.ToInt32(p);
                if(temp > 1)
                    tempFunc(SelectedIndex, temp - 2, ref isFirstTime);
                if(temp == 1) {
                    NavigateProvider.ShopViewScreen().Navigate(AccountStore.instance.CurrentAccount);
                }
                else if(temp == 2) {
                    NavigateProvider.ShopOrderScreen().Navigate();
                }
                else if(temp == 3) {
                    NavigateProvider.ShopProductScreen().Navigate();
                }
                else if(temp == 4) {
                    NavigateProvider.ShopPromoScreen().Navigate();
                }
                else if(temp == 5) {
                    NavigateProvider.ShopRatingScreen().Navigate();
                }
                else {
                    NavigateProvider.ShopStatisticScreen().Navigate();
                }
            });

        }
    }
}
