using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.Media.Playback;

namespace WinRTXamlToolkit.AwaitableUI
{
    /// <summary>
    /// Extension methods for awaiting MediaElement state changes.
    /// </summary>
    public static class MediaPlayerExtensions
    {
        /// <summary>
        /// Waits for the MediaElement.CurrentState to change to any (default) or specific MediaElementState value.
        /// </summary>
        /// <param name="mediaElement"></param>
        /// <param name="newState">The MediaElementState value to wait for. Null by default causes the metod to wait for a change to any other state.</param>
        /// <returns></returns>
        public static async Task<MediaPlayer> WaitForStateAsync(this MediaPlayer mediaElement, MediaPlayerState? newState = null)
        {
            if (newState != null &&
                mediaElement.CurrentState == newState.Value)
            {
                return null;
            }

            var tcs = new TaskCompletionSource<MediaPlayer>();
            TypedEventHandler<MediaPlayer, object> reh = null;

            reh = (s, e) =>
            {
                if (newState != null && mediaElement.CurrentState != newState.Value)
                {
                    return;
                }

                mediaElement.CurrentStateChanged -= reh;
                tcs.SetResult(s);
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
        public static async Task<MediaPlayer> PlayToEndAsync(this MediaPlayer mediaElement, Uri source)
        {
            mediaElement.SetUriSource(source);
            return await mediaElement.WaitToCompleteAsync();
        }

        /// <summary>
        /// Waits for the MediaElement to complete playback.
        /// </summary>
        /// <param name="mediaElement">The media element.</param>
        /// <returns></returns>
        public static async Task<MediaPlayer> WaitToCompleteAsync(this MediaPlayer mediaElement)
        {
            //if (mediaElement.CurrentState != MediaElementState.Closed &&
            //    mediaElement.CurrentState != MediaElementState.Buffering &&
            //    mediaElement.CurrentState != MediaElementState.Opening &&
            //    mediaElement.CurrentState != MediaElementState.Playing)
            //{
            //    return mediaElement;
            //}

            var tcs = new TaskCompletionSource<MediaPlayer>();
            TypedEventHandler<MediaPlayer, object> reh = null;

            reh = (s, e) =>
            {
                if (mediaElement.CurrentState == MediaPlayerState.Buffering ||
                    mediaElement.CurrentState == MediaPlayerState.Opening ||
                    mediaElement.CurrentState == MediaPlayerState.Playing)
                {
                    return;
                }

                mediaElement.CurrentStateChanged -= reh;
                tcs.SetResult(s);
            };

            mediaElement.CurrentStateChanged += reh;

            return await tcs.Task;
        }
    }
}
