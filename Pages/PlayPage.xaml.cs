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
using System.Diagnostics;
using ToastNotifications.Messages;
using System.IO;
using ToastNotifications.Lifetime.Clear;

namespace MultiGDLauncher
{
    /// <summary>
    /// Interaction logic for PlayPage.xaml
    /// </summary>
    public partial class PlayPage : Page
    {
        public PlayPage(string? initName = null)
        {
            InitializeComponent();
            initInstancesList();

            iconsList.Visibility = Visibility.Collapsed;
            instanceSettings.Visibility = Visibility.Collapsed;

            if (instancesCombo.Items.Count > 0)
            {
                instancesCombo.SelectedIndex = 0;
            }

            if (initName != null)
            {
                selectInstanceByName(initName);
            }
        }

        private void selectInstanceByName(string instanceName)
        {
            for (int i = 0; i < instancesCombo.Items.Count; i++)
            {
                var newItem = (GDInstanceItem)instancesCombo.Items.GetItemAt(i);
                if (newItem.Info.Name == instanceName)
                {
                    instancesCombo.SelectedIndex = i;
                    break;
                }
            }
        }

        private void initInstancesList()
        {
            instancesCombo.Items.Clear();
            var instances = Utils.GetInstances();

            foreach ( var instance in instances )
            {
                var elem = new GDInstanceItem(instance.Name, instance);
                instancesCombo.Items.Add(elem);
            }
        }

        private void iconListItem_Click(object sender, EventArgs e)
        {
            IconListViewItem item = (IconListViewItem)sender;
            iconsList.Visibility = Visibility.Collapsed;

            GDInstanceItem instanceItem = (GDInstanceItem)instancesCombo.SelectedItem;
            int oldIndex = instancesCombo.SelectedIndex;

            if (instanceItem == null)
            {
                return;
            }

            var info = instanceItem.Info;

            info.LocalIconName = item.Filename;

            Utils.SetInstanceInfo($"{LauncherSettingsState.StoragePath}\\{instanceItem.Info.Name}", info);
            initInstancesList();

            instancesCombo.SelectedIndex = oldIndex;

            // currentIconImage.Source = item.IconSource;
            // selectedIcon = item.Filename;
        }

        private void constructDefaultImagesList()
        {
            iconsList.Items.Clear();

            for (int i = 0; i < Utils.DefaultIconsCount; i++)
            {
                IconListViewItem item = new IconListViewItem();
                item.Width = 46;
                item.Height = 46;
                item.Padding = new Thickness(4, 1, 6, 1);
                item.MinWidth = 46;
                item.MinHeight = 46;
                item.MaxWidth = 46;
                item.MaxHeight = 46;
                item.Margin = new Thickness(10, 0, 10, 0);
                item.HorizontalContentAlignment = HorizontalAlignment.Center;
                item.Cursor = Cursors.Hand;
                item.Selected += iconListItem_Click;

                Image image = new Image();
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.StreamSource = Application.GetResourceStream(new Uri($"/Assets/ProfilesIcons/{i}.png", UriKind.Relative)).Stream;
                bi3.EndInit();
                image.Source = bi3;

                item.Content = image;
                item.IconSource = bi3;
                item.Filename = $"{i}.png";

                iconsList.Items.Add(item);

            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            GDInstanceItem instanceItem = (GDInstanceItem)instancesCombo.SelectedItem;
            
            if ( instanceItem == null )
            {
                return;
            }

            var startInfo = new ProcessStartInfo();

            string gameExecutable = instanceItem.Info.GameExecutable != null ? instanceItem.Info.GameExecutable : "GeometryDash.exe";

            startInfo.WorkingDirectory = $"{LauncherSettingsState.StoragePath}\\{instanceItem.Info.Name}";
            startInfo.FileName = $"{LauncherSettingsState.StoragePath}\\{instanceItem.Info.Name}\\{gameExecutable}";

            Utils.SharedNotifier.ShowInformation($"Starting {gameExecutable}...");

            try
            {
                Process.Start(startInfo);
            }
            catch (System.ComponentModel.Win32Exception ex) {
                Utils.SharedNotifier.ShowError(ex.Message);
            }

        }

        private void openFolderButton_Click(object sender, RoutedEventArgs e)
        {
            GDInstanceItem instanceItem = (GDInstanceItem)instancesCombo.SelectedItem;

            if (instanceItem == null)
            {
                return;
            }

            var startInfo = new ProcessStartInfo();
            startInfo.FileName = $"explorer";
            startInfo.Arguments = $"{LauncherSettingsState.StoragePath}\\{instanceItem.Info.Name}";

            try
            {
                Process.Start(startInfo);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Utils.SharedNotifier.ShowError($"Unable to open Explorer: {ex.Message}");
            }

        }

        private void changeIconButton_Click(object sender, RoutedEventArgs e)
        {
            iconsList.Visibility = iconsList.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            constructDefaultImagesList();
        }

        private void changeNameButton_Click(object sender, RoutedEventArgs e)
        {
            GDInstanceItem instanceItem = (GDInstanceItem)instancesCombo.SelectedItem;

            if (instanceItem == null)
            {
                return;
            }

            var info = instanceItem.Info;

            string oldName = info.Name;
            string newName = nameInput.Text.Trim();

            if (newName.Length < 3)
            {
                Utils.SharedNotifier.ShowError("Name must be at least 3 characters long!");
                return;
            }

            info.Name = newName;

            try
            {
                Directory.Move($"{LauncherSettingsState.StoragePath}\\{oldName}", $"{LauncherSettingsState.StoragePath}\\{newName}");
            }
            catch (Exception ex)
            {
                Utils.SharedNotifier.ShowError($"Unable to rename directory: {ex.Message}");
            }

            Utils.SetInstanceInfo($"{LauncherSettingsState.StoragePath}\\{newName}", info);
            initInstancesList();

            selectInstanceByName(newName);
        }

        private void instancesCombo_Selected(object sender, SelectionChangedEventArgs e)
        {
            GDInstanceItem instanceItem = (GDInstanceItem)instancesCombo.SelectedItem;

            if (instanceItem == null)
            {
                return;
            }

            instanceSettings.Visibility = Visibility.Visible;

            nameInput.Text = instanceItem.Info.Name;
        }

        private void duplicateButton_Click(object sender, RoutedEventArgs e)
        {
            GDInstanceItem instanceItem = (GDInstanceItem)instancesCombo.SelectedItem;

            if (instanceItem == null)
            {
                return;
            }

            CreateInstancePage page = new CreateInstancePage(
                $"{LauncherSettingsState.StoragePath}\\{instanceItem.Info.Name}",
                $"{instanceItem.Info.Name} (Copy)",
                instanceItem.Info.GameVersion,
                instanceItem.Info.LocalIconName
            );

            Utils.GetMainWindow().navFrame.Navigate(page);
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            GDInstanceItem instanceItem = (GDInstanceItem)instancesCombo.SelectedItem;

            if (instanceItem == null)
            {
                return;
            }

            var result = MessageBox.Show("Are you sure?", "Delete instance", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            Utils.SharedNotifier.ShowInformation("Deleting...");
            try
            {
                Task.Run(() =>
                {
                    Directory.Delete($"{LauncherSettingsState.StoragePath}\\{instanceItem.Info.Name}", true);


                    Dispatcher.Invoke(() =>
                    {
                        Utils.SharedNotifier.ShowSuccess("Done!");

                        instancesCombo.Items.Clear();
                        initInstancesList();

                        instancesCombo.SelectedIndex = 0;
                    });
                });
            }
            catch (Exception ex)
            {
                Utils.SharedNotifier.ShowError($"Error: {ex.Message}");
                return;
            }
        }
    }
}
