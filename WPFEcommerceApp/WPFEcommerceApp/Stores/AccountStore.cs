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
        public static AccountStore instance;
        public event Action AccountChanged;
        public event Action AccountUpdated;

        private MUser currentAccount;
        private bool notInvoke { get; set; } = false;
        public MUser CurrentAccount {
            get { return currentAccount; }
            set {
                currentAccount = value;
                if(!notInvoke)
                    OnCurrentAccountChange();
            }
        }

        private GenericDataRepository<MUser> userRepo = new GenericDataRepository<MUser>();

        public async Task Update(MUser user) {
            notInvoke = true;
            CurrentAccount = user;
            await userRepo.Update(CurrentAccount);
            notInvoke = false;
            AccountUpdated?.Invoke();
        }
        private void OnCurrentAccountChange() {
            AccountChanged?.Invoke();
            var nav = NavigationStore.instance.stackScreen;
            var temp = nav[nav.Count - 1];
            nav.Clear();
            nav.Add(temp);
        }
    }
}
