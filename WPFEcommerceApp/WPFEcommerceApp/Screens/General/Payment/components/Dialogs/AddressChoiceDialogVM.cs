using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DataAccessLayer;
using MaterialDesignThemes.Wpf;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp {
    public class AddressChoiceDialogVM : BaseViewModel {
        private readonly GenericDataRepository<Address> addressRepo = new GenericDataRepository<Address>();
        public ObservableCollection<Address> ListAddress { get; set; } 
        public ICommand ChangeAddressHandle { get; set; }
        public ICommand AddAddressHandle { get; set; }
        public ICommand OnChangeAddress { get; set; }
        public ICommand OnAddAddress { get; set; }
        public Address SelectedItem { get; set; }
        public AddressChoiceDialogVM() {
            OnChangeAddress = new ImmediateCommand<object>(p => {
                ChangeAddressHandle.Execute(p);
                DialogHost.Close("Main");
            });
            OnAddAddress = new ImmediateCommand<object>(p => {
                var dl = new EditInforDialog() {
                    OnAddAddress = new ImmediateCommand<object>(async o => {
                        await addressRepo.Add(o as Address);
                        ListAddress.Add(o as Address);
                        SelectedItem = o as Address;
                        AddAddressHandle.Execute(o);
                    })
                };
                DialogHost.Show(dl, "AddressCheckout");
            });
        }
    }
}
