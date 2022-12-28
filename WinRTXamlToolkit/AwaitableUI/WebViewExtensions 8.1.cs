using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.UI.Xaml.Controls;

namespace WinRTXamlToolkit.AwaitableUI
{
    /// <summary>
    /// Extension methods for WebView class.
    /// </summary>
    public static class WebViewExtensions
    {
        /// <summary>
        /// Navigates to the given source URI and waits for the loading to complete or fail.
        /// </summary>
        public static async Task NavigateAsync(this WebView2 webView, Uri source)
        {
            var tcs = new TaskCompletionSource<object>();

            TypedEventHandler<WebView2, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs> nceh = null;

            nceh = (s, e) =>
            {
                webView.NavigationCompleted -= nceh;
                tcs.SetResult(null);
            };

            webView.NavigationCompleted += nceh;
            webView.CoreWebView2.Navigate(source.ToString());

            await tcs.Task; 
        }
    }
}
