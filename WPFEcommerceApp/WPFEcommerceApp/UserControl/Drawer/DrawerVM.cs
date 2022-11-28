using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEcommerceApp{
    public class DrawerVM : BaseViewModel {
        private ObservableCollection<ButtonItem> _buttonItems = new ObservableCollection<ButtonItem> {
            new ButtonItem(),
            new ButtonItem(),
            new ButtonItem(),
            new ButtonItem("Gear", "Settings"),
        };
        public ObservableCollection<ButtonItem> ButtonItems {
            get => _buttonItems;
            set {
                _buttonItems = value;
                OnPropertyChanged();
            }
        }
        private int selectedIndex = 0;

        public int SelectedIndex {
            get { return selectedIndex = 0; }
            set { selectedIndex = value; OnPropertyChanged(); }
        }

    }
    public class ButtonItem {
        public string Icon { get; set; }
        public string Text { get; set; }
        public string NavLink { get; set; }
        public ButtonItem() {
            Icon = "Home";
            Text = "Home";
            NavLink="/Home.xaml";
        }
        public ButtonItem(string icon, string text, string navLink="") {
            Icon=icon;
            Text=text;
            NavLink=navLink;
        }
    }
}
