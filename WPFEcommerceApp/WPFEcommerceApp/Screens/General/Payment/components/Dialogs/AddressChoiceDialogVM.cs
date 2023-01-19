using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        public ICommand OnEditAddress { get; set; }
        public Address SelectedItem { get; set; }
        public AddressChoiceDialogVM() {
            OnChangeAddress = new RelayCommand<object>(p => SelectedItem != null, p => {
                ChangeAddressHandle.Execute(p);
                DialogHost.Close("Main");
            });
            OnAddAddress = new ImmediateCommand<object>(async p => {
                var Main = MainViewModel.UpdateDialog("Main");

                var dl = new EditInforDialog() {
                    DataContext = new EditInforDialogVM() {
                        OnOK = new ImmediateCommand<object>(async o => {
                            await addressRepo.Add(o as Address);
                            ListAddress.Add(o as Address);
                            SelectedItem = o as Address;
                            AddAddressHandle.Execute(o);
                            CommandManager.InvalidateRequerySuggested();
                        }),
                        PrevDialog = Main
                    }
                };

                await DialogHost.Show(dl, "Main");
            });
            OnEditAddress = new ImmediateCommand<object>(p => {
                var Main = MainViewModel.UpdateDialog("Main");

                var dl = new EditInforDialog() {
                    DataContext = new EditInforDialogVM(p as Address) {
                        OnOK = new ImmediateCommand<object>(async o => {
                            await addressRepo.Update(o as Address);
                            for(int i = 0; i < ListAddress.Count; i++) {
                                if(ListAddress[i].Id == (o as Address).Id) {
                                    ListAddress.RemoveAt(i);
                                    ListAddress.Insert(i, o as Address);
                                    SelectedItem = o as Address;
                                    break;
                                }
                            }
                            AddAddressHandle.Execute(o);
                        }),
                        PrevDialog = Main
                    },
                    Header = "Edit address"
                };
                DialogHost.Show(dl, "Main");
            });
        }
    }
}
