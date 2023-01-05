using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFEcommerceApp {
    /// <summary>
    /// Interaction logic for LoadingScreen.xaml
    /// </summary>
    public partial class LoadingScreen : UserControl {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        public ICommand PlayAudio { get; set; }
        public ICommand StopAudio { get; set; }

        public LoadingScreen() {
            InitializeComponent();
            DataContext = this;
            mediaPlayer.Volume = 50;
            PlayAudio = new RelayCommand<object>(p => true, p => {
                mediaPlayer.Open(new Uri(@"..\..\Assets\Sounds\LoadingSound.mp3", uriKind: UriKind.RelativeOrAbsolute));
                mediaPlayer.Play();
            });

            StopAudio = new RelayCommand<object>(p => true, p => {
                mediaPlayer.Close();
            });
        }

    }
}
