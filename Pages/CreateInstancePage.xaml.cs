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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using ToastNotifications.Utilities;

namespace MultiGDLauncher
{
    /// <summary>
    /// Interaction logic for CreateInstancePage.xaml
    /// </summary>
    public partial class CreateInstancePage : Page
    {
        private string selectedIcon = "0.png";

        public CreateInstancePage(
            string originalPath, 
            string proposedName, 
            string proposedVersion = "2.204", 
            string proposedIcon = "0.png"
        )
        {
            InitializeComponent();
            choosePathFromInput.Text = originalPath;
            instanceNameInput.Text = proposedName;

            int versionIndex = 0;
            int index = 0;

            foreach(ComboBoxItem item in versionCombo.Items)
            {
                if (item.Content.ToString().Trim() == proposedVersion)
                {
                    versionIndex = index;
                    break;
                }

                index++;
            }

            versionCombo.SelectedIndex = versionIndex;
            iconsList.Visibility = Visibility.Collapsed;
            selectedIcon = proposedIcon;
            goPlayBtn.Visibility = Visibility.Collapsed;

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.StreamSource = Application.GetResourceStream(
                new Uri($"/Assets/ProfilesIcons/{selectedIcon}", UriKind.Relative)
            ).Stream;
            bi3.EndInit();
            currentIconImage.Source = bi3;
        }

        private void iconListItem_Click(object sender, EventArgs e)
        {
            IconListViewItem item = (IconListViewItem)sender;
            iconsList.Visibility = Visibility.Collapsed;
            currentIconImage.Source = item.IconSource;
            selectedIcon = item.Filename;
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
                bi3.StreamSource = Application.GetResourceStream(
                    new Uri($"/Assets/ProfilesIcons/{i}.png", UriKind.Relative)
                ).Stream;
                bi3.EndInit();
                image.Source = bi3;

                item.Content = image;
                item.IconSource = bi3;
                item.Filename = $"{i}.png";

                iconsList.Items.Add(item);

            }
        }

        private void choosePathFromButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result != CommonFileDialogResult.Ok)
            {
                return;
            }

            choosePathFromInput.Text = dialog.FileName;
        }

        private string constructInstallationPath()
        {
            return Utils.GetInstanceDirectory(instanceNameInput.Text);
        }

        private void onEndedCopying()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (Utils.GetIsWindowFocused())
                {
                    Utils.SharedNotifier.ShowSuccess("Done cloning!");
                }
                else
                {
                    Utils.SendPushNotification("Done cloning!", System.Windows.Forms.ToolTipIcon.None);
                }
                cloneButton.IsEnabled = true;
                cloneButton.Content = "Clone";
                this.IsEnabled = true;
                this.Opacity = 1;
                Utils.EndedCopying -= onEndedCopying;
                goPlayBtn.Visibility = Visibility.Visible;

                PlayPage playPage = new PlayPage(instanceNameInput.Text);
                Utils.GetMainWindow().navFrame.Navigate(playPage);

            }), null);
        }

        private void cloneButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                $"Do you want to clone GD to {constructInstallationPath()}?", 
                "Clone GD", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question
            );

            ComboBoxItem comboBoxItem = (ComboBoxItem)versionCombo.Items[versionCombo.SelectedIndex];

            if (result == MessageBoxResult.Yes)
            {
                Utils.EndedCopying += onEndedCopying;
                Utils.SharedNotifier.ShowInformation("Cloning...");

                cloneButton.Content = "Cloning...";
                cloneButton.IsEnabled = false;

                this.IsEnabled = false;
                this.Opacity = 0.5;

                Utils.CloneGD(
                    choosePathFromInput.Text,
                    constructInstallationPath(),
                    new InstanceInfo(
                        instanceNameInput.Text,
                        comboBoxItem.Content.ToString(),
                        selectedIcon,
                        instanceExecInput.Text
                    )
                );
            }
        }

        private void changeIconButton_Click(object sender, RoutedEventArgs e)
        {
            iconsList.Visibility = iconsList.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            constructDefaultImagesList();
        }

        private void goPlayBtn_Clicked(object sender, RoutedEventArgs e)
        {
            Utils.GetMainWindow().OnPlay(sender, e);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Utils.GetMainWindow().navFrame.GoBack();
        }
    }
}
