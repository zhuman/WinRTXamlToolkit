using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRTXamlToolkit.Debugging.ViewModels;

namespace WinRTXamlToolkit.Debugging.Views
{
    public sealed partial class DependencyObjectDetailsView : UserControl
    {
        public DependencyObjectDetailsView()
        {
            this.InitializeComponent();
        }

        private void PreviewTabButton_OnChecked(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as DependencyObjectViewModel;

            if (vm == null)
            {
                return;
            }

            vm.VisualTreeViewModel.IsPreviewShown = true;
#pragma warning disable 4014
            vm.LoadPreviewAsync();
#pragma warning restore 4014
        }

        private void PreviewTabButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as DependencyObjectViewModel;

            if (vm == null)
            {
                return;
            }

            vm.VisualTreeViewModel.IsPreviewShown = false;
        }
    }
}
