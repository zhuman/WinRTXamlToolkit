using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinRTXamlToolkit.Sample.Views
{
    public sealed partial class ButtonBaseExtensionsTestView : UserControl
    {
        public ButtonBaseExtensionsTestView()
        {
            this.InitializeComponent();
        }

        private void ButtonStateEventBehavior_OnUp(object sender, EventArgs e)
        {
            TestLabel.Visibility = Visibility.Collapsed;
        }

        private void ButtonStateEventBehavior_OnDown(object sender, EventArgs e)
        {
            TestLabel.Visibility = Visibility.Visible;
        }
    }
}
