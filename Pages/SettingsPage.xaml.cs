using System;
using System.Collections.Generic;
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
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;

namespace MultiGDLauncher
{

    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {

        public SettingsPage()
        {
            InitializeComponent();
            storageDirectoryInput.Text = LauncherSettingsState.StoragePath;
            steamGDDirectoryInput.Text = LauncherSettingsState.SteamGDPath;
        }

        private void storageDirectorySaveButton_Click(object sender, RoutedEventArgs e)
        {
            LauncherSettingsState.StoragePath = storageDirectoryInput.Text;
            Utils.SharedNotifier.ShowSuccess("Done!");
            this.Focus();
        }

        private void storageDirectoryInput_keyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                storageDirectorySaveButton_Click(sender, e);
            }
        }

        private void steamGDDirectoryInput_keyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                steamGDDirectorySaveButton_Click(sender, e);
            }
        }

        private void steamGDDirectorySaveButton_Click(object sender, RoutedEventArgs e)
        {
            LauncherSettingsState.SteamGDPath = steamGDDirectoryInput.Text;
            Utils.SharedNotifier.ShowSuccess("Done!");
            this.Focus();
        }
    }
}
