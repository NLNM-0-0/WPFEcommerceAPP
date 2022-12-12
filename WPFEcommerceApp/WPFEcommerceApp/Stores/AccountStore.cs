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

        private GenericDataRepository<MUser> userRepo = new GenericDataRepository<MUser>();

        public async Task Update(MUser user) {
            CurrentAccount = user;
            await userRepo.Update(CurrentAccount);
        }
        public async Task Add(MUser user) {
            throw new NotImplementedException();

        }
        public async Task Remove(MUser user) {
            throw new NotImplementedException();
        }
        private void OnCurrentAccountChange() {
            AccountChanged?.Invoke();
        }
    }
}
