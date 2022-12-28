using WinRTXamlToolkit.Debugging.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;

namespace WinRTXamlToolkit.Debugging.Views
{
    public sealed partial class PropertiesView : UserControl
    {
        private DispatcherTimer _updatePropertyNameFilterDelayTimer = new DispatcherTimer();
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesView"/> class.
        /// </summary>
        public PropertiesView()
        {
            this.InitializeComponent();
            _updatePropertyNameFilterDelayTimer.Interval = TimeSpan.FromSeconds(0.5);
            _updatePropertyNameFilterDelayTimer.Tick += OnSearchBoxTextChangedCommitTime;
        }

        private void OnSearchBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            _updatePropertyNameFilterDelayTimer.Start();
        }

        private void OnSearchBoxTextChangedCommitTime(object sender, object e)
        {
            _updatePropertyNameFilterDelayTimer.Stop();
            var vm = (DependencyObjectViewModel)this.DataContext;

            if (vm != null)
            {
                vm.PropertyNameFilter = this.SearchBox.Text;
            }
        }

        private void Border_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
