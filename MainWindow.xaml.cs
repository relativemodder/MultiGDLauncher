using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultiGDLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }
        public void WindowLoaded(object sender, EventArgs e)
        {
            Utils.InitNotifier();
        }

        public void OnSettings(object sender, RoutedEventArgs e)
        {
            SettingsPage settingsPage = new SettingsPage();
            navFrame.Navigate(settingsPage);
        }

        public void OnPlay(object sender, RoutedEventArgs e)
        {
            PlayPage playPage = new PlayPage();
            navFrame.Navigate(playPage);
        }

        public void OnInstall(object sender, RoutedEventArgs e)
        {
            InstallPage installPage = new InstallPage(this);
            navFrame.Navigate(installPage);
        }
    }
}
