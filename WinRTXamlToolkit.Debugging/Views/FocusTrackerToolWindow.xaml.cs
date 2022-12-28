using System;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRTXamlToolkit.Debugging.ViewModels;

namespace WinRTXamlToolkit.Debugging.Views
{
    public sealed partial class FocusTrackerToolWindow : UserControl
    {
        private FocusTrackerToolWindowViewModel vm;

        public FocusTrackerToolWindow()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this.vm = (FocusTrackerToolWindowViewModel)this.DataContext;
            vm.FocusTracker = this.FocusVisualizer.FocusTracker;
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            vm.FocusTracker = null;
        }

        private void Window_OnClosing(object sender, CancelEventArgs e)
        {
            var vm = (FocusTrackerToolWindowViewModel)this.DataContext;
            vm.Remove();
            //((ToolWindow)sender).Hide();
            //e.Cancel = true;
        }

        private void OnSelectedEventChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.FocusEventsListView.SelectedItem != null)
            {
                this.FocusEventsListView.ScrollIntoView(this.FocusEventsListView.SelectedItem);
            }
        }
    }
}
