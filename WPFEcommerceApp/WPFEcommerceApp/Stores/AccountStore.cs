using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    public class AccountStore {
        public event Action AccountChanged;
        private MUser currentAccount;

        public MUser CurrentAccount {
            get { return currentAccount; }
            set {
                currentAccount = value;
                OnCurrentAccountChange();
            }
        }
        public void Change(MUser user) {
            CurrentAccount = user;
        }
        private void OnCurrentAccountChange() {
            AccountChanged?.Invoke();
        }
    }
}
