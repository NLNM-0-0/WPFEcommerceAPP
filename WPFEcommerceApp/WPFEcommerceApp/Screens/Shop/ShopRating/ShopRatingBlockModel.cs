using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using WPFEcommerceApp.Models;

namespace WPFEcommerceApp
{
    public class ShopRatingBlockModel : BaseViewModel
    {
        public ICommand OpenReplyCommand { get; set; }
        public ICommand OpenAllCommands { get; set; }
        public ICommand ReplyCommand { get; set; }
        private Models.OrderInfo orderInfo;
        public Models.OrderInfo OrderInfo
        {
            get => orderInfo;
            set
            {
                orderInfo = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(SourceImageAva));
                OnPropertyChanged(nameof(ImageProduct));
                OnPropertyChanged(nameof(TimeRating));
            }
        }
        public string SourceImageAva
        {
            get
            {
                if (OrderInfo == null || OrderInfo.MOrder.MUser == null || String.IsNullOrEmpty(OrderInfo.MOrder.MUser.SourceImageAva))
                {
                    return Properties.Resources.DefaultShopAvaImage;
                }
                else
                {
                    return OrderInfo.MOrder.MUser.SourceImageAva;
                }
            }
        }
        public string ImageProduct
        {
            get
            {
                if (OrderInfo == null || OrderInfo.Product.ImageProducts == null || OrderInfo.Product.ImageProducts.Count == 0)
                {
                    return Properties.Resources.DefaultProductImage;
                }
                else
                {
                    return OrderInfo.Product.ImageProducts.ElementAt(0).Source;
                }
            }
        }
        public string TimeRating
        {
            get
            {
                return OrderInfo.Rating.DateRating.Value.ToShortDateString();
            }
        }
        private bool isReplying;
        public bool IsReplying
        {
            get => isReplying;
            set
            {
                isReplying = value;
                OnPropertyChanged();
            }
        }
        private bool isShowAll;
        public bool IsShowAll
        {
            get => isShowAll;
            set
            {
                isShowAll = value;
                OnPropertyChanged();
            }
        }
        private AddNewReplyBlockViewModel newRelayblockViewModel;
        public AddNewReplyBlockViewModel NewRelayblockViewModel
        {
            get => newRelayblockViewModel;
            set
            {
                newRelayblockViewModel = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ReplyBlockViewModel> displayedBlocksViewModels;
        public ObservableCollection<ReplyBlockViewModel> DisplayedBlocksViewModels
        {
            get => displayedBlocksViewModels;
            set
            {
                displayedBlocksViewModels = value;
                OnPropertyChanged();
            }
        }
        public ShopRatingBlockModel(Models.OrderInfo orderInfo)
        {
            OrderInfo = orderInfo;
            IsReplying = false;
            OpenReplyCommand = new RelayCommandWithNoParameter(() =>
            {
                if (!IsReplying)
                {
                    IsReplying = true;
                }
                else
                {
                    IsReplying = false;
                    NewRelayblockViewModel.Comment = "";
                }    
            });
            ReplyCommand = new RelayCommand<object>((p)=>
            {
                return !String.IsNullOrEmpty(NewRelayblockViewModel.Comment);
            },(async (p) =>
            {
                MainViewModel.IsLoading = true;
                await ReplyComment();
                MainViewModel.IsLoading = false;
            }));
            OpenAllCommands = new RelayCommandWithNoParameter(() =>
            {
                Task.Run(() =>
                {

                }).ContinueWith((temp) =>
                {
                    MainViewModel.IsLoading = true;
                    IsShowAll = false;
                    int index = DisplayedBlocksViewModels.Count;
                    for (int i = 0; i < OrderInfo.Rating.RatingInfoes.Count - 1; i++)
                    {
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            DisplayedBlocksViewModels.Insert(index, new ReplyBlockViewModel(OrderInfo.Rating.RatingInfoes.ElementAt(i)));
                        }));
                    }
                    MainViewModel.IsLoading = false;
                });
                
            });
            NewRelayblockViewModel = new AddNewReplyBlockViewModel();
            NewRelayblockViewModel.User = AccountStore.instance.CurrentAccount;
            NewRelayblockViewModel.Comment = "";
            NewRelayblockViewModel.ReplyComment = ReplyCommand;
            if(OrderInfo.Rating.RatingInfoes.Count == 0)
            {
                DisplayedBlocksViewModels = new ObservableCollection<ReplyBlockViewModel>();
                IsShowAll = false;
            }
            else if (OrderInfo.Rating.RatingInfoes.Count == 1)
            {
                DisplayedBlocksViewModels = new ObservableCollection<ReplyBlockViewModel>();
                DisplayedBlocksViewModels.Add(new ReplyBlockViewModel(OrderInfo.Rating.RatingInfoes.Last()));
                IsShowAll = false;
            }
            else
            {
                DisplayedBlocksViewModels = new ObservableCollection<ReplyBlockViewModel>();
                DisplayedBlocksViewModels.Add(new ReplyBlockViewModel(OrderInfo.Rating.RatingInfoes.Last()));
                IsShowAll = true;
            }
        }   
        public async Task ReplyComment()
        {
            GenericDataRepository<Models.RatingInfo> ratingInfoRepository = new GenericDataRepository<RatingInfo>();
            Models.RatingInfo ratingInfo = new RatingInfo()
            {
                IdUser = AccountStore.instance.CurrentAccount.Id,
                DateReply = DateTime.Now,
                Comment = NewRelayblockViewModel.Comment,
                IdRating = OrderInfo.IdRating,
            };
            await ratingInfoRepository.Add(ratingInfo);
            ratingInfo.MUser = AccountStore.instance.CurrentAccount;
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                DisplayedBlocksViewModels.Insert(0, new ReplyBlockViewModel(ratingInfo));
                IsReplying = false;
                NewRelayblockViewModel.Comment = "";
            }));
        }
    }
}
