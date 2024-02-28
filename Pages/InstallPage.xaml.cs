using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PEUtils;
using ToastNotifications.Messages;

namespace MultiGDLauncher
{
    /// <summary>
    /// Interaction logic for InstallPage.xaml
    /// </summary>
    public partial class InstallPage : Page
    {
        private Random random;

        public InstallPage(MainWindow parentWindow)
        {
            InitializeComponent();
            random = new Random();
        }

        private void cloneFromSteamButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(LauncherSettingsState.SteamGDPath))
            {
                Utils.SharedNotifier.ShowError("Couldn't find Steam GD path, you can change it in Settings Tab!");
                return;
            }

            string gdVer = Utils.GetGDVersion($"{LauncherSettingsState.SteamGDPath}\\GeometryDash.exe");

            CreateInstancePage page = new CreateInstancePage(
                LauncherSettingsState.SteamGDPath, 
                $"Steam_{gdVer}_{random.Next(1111, 9999)}",
                gdVer
            );

            Utils.GetMainWindow().navFrame.Navigate(page);
        }

        private void cloneManuallyButton_Click(object sender, RoutedEventArgs e)
        {
            CreateInstancePage page = new CreateInstancePage("", "", "Unknown");
            Utils.GetMainWindow().navFrame.Navigate(page);
        }
    }
}
