using WinRTXamlToolkit.Debugging.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinRTXamlToolkit.Debugging.Views
{
    public class VisualTreeViewItemDetailsTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PropertyDetailsTemplate { get; set; }
        public DataTemplate DefaultDetailsTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is DependencyObjectViewModel)
            {
                return PropertyDetailsTemplate;
            }

            return DefaultDetailsTemplate;
        }
    }
}
