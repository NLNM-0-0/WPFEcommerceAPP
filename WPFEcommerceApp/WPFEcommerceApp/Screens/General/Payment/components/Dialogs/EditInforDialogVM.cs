using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    public class EditInforDialogVM : BaseViewModel {

        private string _name;
        public string Name { 
            get => _name;
            set {
                _name = null;
                if(string.IsNullOrEmpty(value)) {
                    return;
                }
                _name = value;
            } 
        }
        string _phone;
        public string Phone {
            get => _phone;
            set {
                _phone = null;
                if(string.IsNullOrEmpty(value)) {
                    return;
                }
                if(value.Length <= 6 || value.Length >= 12) {
                    throw new ArgumentException("*Wrong type");
                }
                if(!ValidateRegex.Phone.IsMatch(value)) {
                    throw new ArgumentException("*Wrong type");
                }
                _phone = value;
            }
        }
        string _address;
        public string Address {
            get => _address;
            set {
                _address = null;
                if(string.IsNullOrEmpty(value)) {
                    return;
                }
                if(value.Length < 10) {
                    throw new ArgumentException("*Too short");
                }
                _address = value;
            }
        }
        public Address address { get; set; }
        public ICommand ConfirmCM { get; set; }
        public ICommand OnOK { get; set; }
        public EditInforDialogVM(Address mAdd = null) {
            address = mAdd;
            this.Name = address?.Name;
            this.Phone = address?.PhoneNumber;
            this.Address = address?.Address1;

            ConfirmCM = new RelayCommand<object>(p => {
                return !string.IsNullOrEmpty(Name) &&
                !string.IsNullOrEmpty(Address) &&
                !string.IsNullOrEmpty(Phone);
            }, p => {
                if(address != null && address.Id != null) {
                    address.Name = Name;
                    address.PhoneNumber = Phone;
                    address.Address1 = Address;
                    OnOK.Execute(address);
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
            });
        }
    }
}
