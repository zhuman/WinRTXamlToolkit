using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace WinRTXamlToolkit.AwaitableUI
{
    /// <summary>
    /// Extension methods for awaiting MediaElement state changes.
    /// </summary>
    public static class MediaElementExtensions
    {
        /// <summary>
        /// Waits for the MediaElement.CurrentState to change to any (default) or specific MediaElementState value.
        /// </summary>
        /// <param name="mediaElement"></param>
        /// <param name="newState">The MediaElementState value to wait for. Null by default causes the metod to wait for a change to any other state.</param>
        /// <returns></returns>
        public static async Task<MediaElement> WaitForStateAsync(this MediaElement mediaElement, MediaElementState? newState = null)
        {
            if (newState != null &&
                mediaElement.CurrentState == newState.Value)
            {
                return null;
            }

            var tcs = new TaskCompletionSource<MediaElement>();
            RoutedEventHandler reh = null;

            reh = (s, e) =>
            {
                if (newState != null && mediaElement.CurrentState != newState.Value)
                {
                    return;
                }

                mediaElement.CurrentStateChanged -= reh;
                tcs.SetResult((MediaElement)s);
            };

            mediaElement.CurrentStateChanged += reh;

            return await tcs.Task;
        }

        /// <summary>
        /// Plays to end and waits asynchronously.
        /// </summary>
        /// <param name="mediaElement">The media element.</param>
        /// <param name="source">The source to play.</param>
        /// <returns></returns>
        public static async Task</*
                TODO UA306_E: UWP MediaElement : Microsoft.UI.Xaml.Controls.MediaElement is not yet supported in WindowsAppSDK. Read: https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/what-is-supported
            */MediaElement> PlayToEndAsync(this MediaElement mediaElement, Uri source)
        {
            mediaElement.Source = source;
            return await mediaElement.WaitToCompleteAsync();
        }

        /// <summary>
        /// Waits for the MediaElement to complete playback.
        /// </summary>
        /// <param name="mediaElement">The media element.</param>
        /// <returns></returns>
        public static async Task<MediaElement> WaitToCompleteAsync(this MediaElement mediaElement)
        {
            //if (mediaElement.CurrentState != MediaElementState.Closed &&
            //    mediaElement.CurrentState != MediaElementState.Buffering &&
            //    mediaElement.CurrentState != MediaElementState.Opening &&
            //    mediaElement.CurrentState != MediaElementState.Playing)
            //{
            //    return mediaElement;
            //}

            var tcs = new TaskCompletionSource<MediaElement>();
            RoutedEventHandler reh = null;

            reh = (s, e) =>
            {
                if (mediaElement.CurrentState == MediaElementState.Buffering ||
                    mediaElement.CurrentState == MediaElementState.Opening ||
                    mediaElement.CurrentState == MediaElementState.Playing)
                {
                    return;
                }

                mediaElement.CurrentStateChanged -= reh;
                tcs.SetResult((MediaElement)s);
            };

            mediaElement.CurrentStateChanged += reh;

            return await tcs.Task;
        }
    }
}
