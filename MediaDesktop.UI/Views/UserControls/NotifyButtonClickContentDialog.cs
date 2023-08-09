using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MediaDesktop.UI.Views.UserControls
{
    public class NotifyButtonClickContentDialogButtonClickEventArgs
    {
        private ContentDialogButtonClickEventArgs baseArgs;

        public ContentDialogResult Result { get; }
        public bool Cancel
        {
            get { return baseArgs.Cancel; }
            set { baseArgs.Cancel = value; }
        }

        public ContentDialogButtonClickDeferral GetDeferral() => baseArgs.GetDeferral();
        public NotifyButtonClickContentDialogButtonClickEventArgs(ContentDialogButtonClickEventArgs baseArgs, ContentDialogResult result)
        {
            this.baseArgs = baseArgs ?? throw new ArgumentNullException(nameof(baseArgs));
            Result = result;
        }
    }
    public class NotifyButtonClickContentDialog : ContentDialog
    {
        static readonly TimeSpan minOpenTimeSpan = TimeSpan.FromMilliseconds(650);
        static DateTime lastOpenTime = DateTime.MinValue;
        static int currentlyOpenedDialogHashCode = 0;

        static SemaphoreSlim minOpenIntervalSemaphore = new SemaphoreSlim(1, 1);
        static SemaphoreSlim lastDialogClosingEventTriggeredSemaphore = new SemaphoreSlim(1, 1);
        public event TypedEventHandler<NotifyButtonClickContentDialog, NotifyButtonClickContentDialogButtonClickEventArgs> ButtonClicked;
        public NotifyButtonClickContentDialog() : base()
        {
            this.CloseButtonClick += NotifyButtonClickedContentDialog_CloseButtonClick;
            this.PrimaryButtonClick += NotifyButtonClickedContentDialog_PrimaryButtonClick;
            this.SecondaryButtonClick += NotifyButtonClickedContentDialog_SecondaryButtonClick;
            this.Opened += NotifyButtonClickContentDialog_Opened;
            this.Closing += NotifyButtonClickContentDialog_Closing;
        }

        private void NotifyButtonClickContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            currentlyOpenedDialogHashCode = 0;
            lastDialogClosingEventTriggeredSemaphore.Release();
        }

        private void NotifyButtonClickContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            currentlyOpenedDialogHashCode = this.GetHashCode();
            lastOpenTime = DateTime.Now;
            Task.Run(() => { Task.Delay(minOpenTimeSpan).Wait(); minOpenIntervalSemaphore.Release(); });
        }

        /// <summary>
        /// Shows the dialog asynchronously. This method can promise the timespan between this show and the last show is at least 650ms. Use this method if you want to immediately show a dialog after hiding another dialog.
        /// </summary>
        public void PromisedShowDeferral(bool throwExceptionOnFailed = false)
        {
            if (currentlyOpenedDialogHashCode == this.GetHashCode())
            {
                if (throwExceptionOnFailed) 
                    throw new InvalidOperationException("Cannnot show a dialog of the same content concurrently.");
                return;
            }
            Task.Run(() =>
            {
                minOpenIntervalSemaphore.Wait();
                lastDialogClosingEventTriggeredSemaphore.Wait();
                DispatcherQueue.TryEnqueue(() => _ = ShowAsync());
            });
        }

        private void NotifyButtonClickedContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            RaiseClickedEvent(args, ContentDialogResult.Secondary);
        }

        private void NotifyButtonClickedContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            RaiseClickedEvent(args, ContentDialogResult.Primary);
        }

        private void NotifyButtonClickedContentDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            RaiseClickedEvent(args, ContentDialogResult.None);
        }

        private void RaiseClickedEvent(ContentDialogButtonClickEventArgs baseArgs, ContentDialogResult result)
        {
            ButtonClicked?.Invoke(this, new NotifyButtonClickContentDialogButtonClickEventArgs(baseArgs, result));
        }
        public override int GetHashCode()
        {
            return Content?.GetType().GetHashCode() ?? -1;
        }
    }
}
