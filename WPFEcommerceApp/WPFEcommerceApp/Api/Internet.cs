using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Threading.Timer;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace WPFEcommerceApp {
    public class Internet {
        public static Internet instance;

        #region checkConnection
        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetGetConnectedState(out int flag, int reserved);
        public static bool CheckConnection() {
            int flag;
            return InternetGetConnectedState(out flag, 0);
        }
        public static bool IsConnected { get; set; }

        #endregion

        #region Check NetworkChanged
        public event EventHandler<EventArgs> NetworkChanged;

        private NetworkInterface[] oldInterfaces;
        private Timer timer;

        public void Start() {
            oldInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            //Invoke a callback every x time (period)
            timer = new Timer(UpdateNetworkStatus, null, new TimeSpan(0, 0, 0, 0, 100), new TimeSpan(0, 0, 0, 0, 500));
        }

        private void UpdateNetworkStatus(object o) {
            var newInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            bool hasChanges = false;
            if(newInterfaces.Length != oldInterfaces.Length) {
                hasChanges = true;
            }
            else {
                for(int i = 0; i < oldInterfaces.Length; i++) {
                    if(oldInterfaces[i].Name != newInterfaces[i].Name || oldInterfaces[i].OperationalStatus != newInterfaces[i].OperationalStatus) {
                        hasChanges = true;
                        break;
                    }
                }
            }

            oldInterfaces = newInterfaces;

            if(hasChanges) {
                RaiseNetworkChanged();
            }
        }

        private void RaiseNetworkChanged() {
            NetworkChanged?.Invoke(this, null);
        }
        #endregion

        #region Navigate handle
        public static void OnlineNav() {
            var nav = NavigationStore.instance.stackScreen;
            var navi = nav[nav.Count - 1];
            if(navi.Item2 == null) navi.Item1.NoBackNavigate();
            else navi.Item1.NoBackNavigate(navi.Item2);
        }

        public static void OfflineNav() {
            NavigateProvider.OfflineScreen().NoBackNavigate();
        }
        #endregion
        public Internet() {
            NetworkChanged += (sender, args) => {
                bool internet = IsConnected;
                bool flag = false;
                var timer = new DispatcherTimer();
                if(!internet) {
                    timer.Interval = TimeSpan.FromMilliseconds(3000);
                    timer.Tick += (sd, e) => {
                        flag = true;
                    };
                    timer.Start();
                }
                while(internet != !IsConnected) {
                    internet = CheckConnection();
                    if(flag) break;
                }
                timer.Stop();
                if(CheckConnection()) {
                    IsConnected = true;
                    OnlineNav();
                }
                else {
                    IsConnected = false;
                    OfflineNav();
                }
                NavigateProvider.LoginScreenHandle(false);
            };
        }
    }
}


