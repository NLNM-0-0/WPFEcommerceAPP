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
        public ShopPopUpVM(
            DrawerVM drawerVM,
            INavigationService ShopMainNavigateService,
            INavigationService ShopOrderNavigateService,
            INavigationService ShopProductNavigateService,
            INavigationService ShopRatingNavigateService) {

            isFirstTime = drawerVM.SelectedIndex == 4;

            SelectedIndex = new ObservableCollection<bool>() { false, false, false };

            var tempFunc = (ObservableCollection<bool> p, int id) => {
                isFirstTime = false;
                for(int i = 0; i < p.Count; i++) {
                    if(id == i) p[i] = true;
                    else p[i] = false;
                }
            };

            OnChecked = new RelayCommand<object>(p => true, p => {
                drawerVM.CanReload = false;
                drawerVM.SelectedIndex = 4;
                var temp = p as string;
                if(temp == "1") {
                    ShopMainNavigateService.Navigate();
                }
                else if(temp == "2") {
                    ShopOrderNavigateService.Navigate();
                    tempFunc(SelectedIndex, 0);
                }
                else if(temp == "3") {
                    ShopProductNavigateService.Navigate();
                    tempFunc(SelectedIndex, 1);
                }
                else {
                    ShopRatingNavigateService.Navigate();
                    tempFunc(SelectedIndex, 2);
                }
            });

        }
    }
}
