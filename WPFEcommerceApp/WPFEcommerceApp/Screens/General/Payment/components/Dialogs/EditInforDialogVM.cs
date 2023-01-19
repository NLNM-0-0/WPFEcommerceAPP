using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    public class EditInforDialogVM : BaseViewModel {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public UserControl PrevDialog { get; set; }
        public Address address { get; set; }
        public ICommand ConfirmCM { get; set; }
        public ICommand OnOK { get; set; }
        public ICommand CloseCM { get; }
        public EditInforDialogVM(Address mAdd = null) {
            address = mAdd;
            this.Name = address?.Name;
            this.Phone = address?.PhoneNumber;
            this.Address = address?.Address1;

            CloseCM = new ImmediateCommand<object>(p => {
                DialogHost.Close("Main");
                DialogHost.Show(PrevDialog, "Main");
            });

            ConfirmCM = new RelayCommand<object>(p => {
                return !string.IsNullOrEmpty(Name) &&
                !string.IsNullOrEmpty(Address) &&
                PhoneValidateRule.Validate(Phone);
            }, p => {
                if(address != null && address.Id != null) {
                    address.Name = Name;
                    address.PhoneNumber = Phone;
                    address.Address1 = Address;
                    OnOK.Execute(address);
                    CloseCM.Execute(null);
                    return;
                }
                var id = GenerateID.DateTimeID();
                Address add = new Address() {
                    Id = id,
                    IdUser = AccountStore.instance.CurrentAccount.Id,
                    Name = this.Name,
                    PhoneNumber = this.Phone,
                    Address1 = this.Address,
                    Status = true
                };
                OnOK.Execute(add);
                CloseCM.Execute(null);
            });
        }
    }
}
