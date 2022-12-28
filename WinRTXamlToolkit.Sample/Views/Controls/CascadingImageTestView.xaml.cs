using WinRTXamlToolkit.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace WinRTXamlToolkit.Sample.Views
{
    public sealed partial class CascadingImageTestView : UserControl
    {
        public CascadingImageTestView()
        {
            this.InitializeComponent();
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            ((CascadingImageControl)sender).Cascade();
        }
    }
}
