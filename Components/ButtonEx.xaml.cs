using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MultiGDLauncher
{
    /// <summary>
    /// Standard button with extensions
    /// </summary>
    public partial class ButtonEx : Button
    {
        readonly static Brush DefaultHoverBackgroundValue = new BrushConverter().ConvertFromString("#FFBEE6FD") as Brush;

        public ButtonEx()
        {
            InitializeComponent();
        }

        public Brush HoverBackground
        {
            get { return (Brush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
        }
        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register(
          "HoverBackground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata(DefaultHoverBackgroundValue));
    }
}