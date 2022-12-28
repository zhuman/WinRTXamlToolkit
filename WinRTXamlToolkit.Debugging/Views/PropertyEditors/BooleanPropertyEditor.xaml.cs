using System;
using WinRTXamlToolkit.Debugging.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinRTXamlToolkit.Debugging.Views.PropertyEditors
{
    public sealed partial class BooleanPropertyEditor : UserControl
    {
        public BooleanPropertyEditor()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var vm = (BasePropertyViewModel)this.DataContext;

            if (vm.PropertyType != typeof(bool?))
            {
                cb.IsThreeState = false;
            }
        }
    }
}
