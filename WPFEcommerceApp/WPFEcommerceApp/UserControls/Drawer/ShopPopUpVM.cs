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

            SelectedIndex = new ObservableCollection<bool>() { false, false, false };

            OnChecked = new RelayCommand<object>(p => true, p => {
                drawerVM.CanReload = false;
                drawerVM.SelectedIndex = 4;
                var temp = p as string;
                if(temp == "1") {
                    //Unknown
                }
                else if(temp == "2") {
                    NavigateProvider.ShopOrderScreen().Navigate();
                    tempFunc(SelectedIndex, 0, ref isFirstTime);
                }
                else if(temp == "3") {
                    NavigateProvider.ShopProductScreen().Navigate();
                    tempFunc(SelectedIndex, 1, ref isFirstTime);
                }
                else {
                    NavigateProvider.ShopRatingScreen().Navigate();
                    tempFunc(SelectedIndex, 2, ref isFirstTime);
                }
            });

        }
    }
}
