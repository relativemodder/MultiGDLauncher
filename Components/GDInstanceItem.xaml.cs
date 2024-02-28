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

namespace MultiGDLauncher
{
    /// <summary>
    /// Interaction logic for GDInstanceItem.xaml
    /// </summary>
    public partial class GDInstanceItem : UserControl
    {
        public string InstanceName {  get; set; }
        public InstanceInfo Info {  get; set; }

        public GDInstanceItem(string name, InstanceInfo info)
        {
            InstanceName = name;
            Info = info;

            InitializeComponent();
            this.DataContext = this;

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.StreamSource = Application.GetResourceStream(new Uri($"/Assets/ProfilesIcons/{Info.LocalIconName}", UriKind.Relative)).Stream;
            bi3.EndInit();
            this.Icon.Source = bi3;
            this.InstanceTitle.Content = $"{name} [{info.GameVersion}]";
        }
    }
}
