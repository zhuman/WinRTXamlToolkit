using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using WinRTXamlToolkit.Controls;
using WinRTXamlToolkit.Debugging;
using WinRTXamlToolkit.Sample.Views;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel.Core;

namespace WinRTXamlToolkit.Sample
{
    public sealed partial class AppShell : UserControl
    {
        public static AlternativeFrame Frame { get; private set; }

        public AppShell(LaunchActivatedEventArgs args)
        {
            this.InitializeComponent();
            InitializeFrame(args);
            this.Loaded += this.OnLoaded;
        }

        public AppShell(FileActivatedEventArgs args)
        {
            this.InitializeComponent();
            InitializeFrame(args);
#pragma warning disable 4014
            this.HandleFileActivationAsync(args);
#pragma warning restore 4014
            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //DC.ShowVisualTree(this);
            //DC.Hide();
        }

        private void InitializeFrame(object args)
        {
            Frame = this.RootFrame;
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Frame.NavigateAsync(typeof(MainPage), args);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            Frame.Navigated += async (s, e) =>
            {

            /*
              
            TODO UA307 Default back button in the title bar does not exist in WinUI3 apps.
            The tool has generated a custom back button in the MainWindow.xaml.cs file.
            Feel free to edit its position, behavior and use the custom back button instead.
            Read: https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/case-study-1#restoring-back-button-functionality
            */SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Frame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;

                // TODO: Review if we still need this custom back button.
                //WindowTitleBar.Instance.IsBackButtonVisible = Frame.CanGoBack;
            };

            /*
              
            TODO UA307 Default back button in the title bar does not exist in WinUI3 apps.
            The tool has generated a custom back button in the MainWindow.xaml.cs file.
            Feel free to edit its position, behavior and use the custom back button instead.
            Read: https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/case-study-1#restoring-back-button-functionality
            */SystemNavigationManager.GetForCurrentView().BackRequested += async (s, e) => await Frame.GoBackAsync();
            WindowTitleBar.Instance.BackButtonClicked += async (s, e) => await Frame.GoBackAsync();
        }

        internal async Task HandleFileActivationAsync(FileActivatedEventArgs e)
        {
            foreach (StorageFile file in e.Files.OfType<StorageFile>())
            {
                DC.Trace(await FileIO.ReadTextAsync(file));
            }
        }
    }
}
