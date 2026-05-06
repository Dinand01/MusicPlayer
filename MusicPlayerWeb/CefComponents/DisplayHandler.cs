using Xilium.CefGlue;
using Avalonia.Controls;
using Avalonia.Threading;

namespace MusicPlayerWeb.CefComponents
{
    /// <summary>
    /// Custom display handler for CefGlue.
    /// </summary>
    internal class DisplayHandler : CefDisplayHandler
    {
        /// <summary>
        /// The current window.
        /// </summary>
        private Avalonia.Controls.Window _currentWindow;

        /// <summary>
        /// The dispatcher for the ui thread.
        /// </summary>
        private Dispatcher _dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayHandler" /> class.
        /// </summary>
        /// <param name="window">The parent window.</param>
        /// <param name="disp">The dispatcher for the ui thread.</param>
        public DisplayHandler(Avalonia.Controls.Window window, Dispatcher disp)
        {
            _currentWindow = window;
            _dispatcher = disp;
        }

        protected override void OnAddressChange(CefBrowser browser, CefFrame frame, string url)
        {
            base.OnAddressChange(browser, frame, url);
        }

        protected override void OnTitleChange(CefBrowser browser, string title)
        {
            base.OnTitleChange(browser, title);
        }

        protected override bool OnConsoleMessage(CefBrowser browser, CefLogSeverity level, string message, string source, int line)
        {
            System.Diagnostics.Debug.WriteLine($"Console: {message}");
            return true;
        }

        protected override void OnStatusMessage(CefBrowser browser, string value)
        {
            base.OnStatusMessage(browser, value);
        }

        protected override void OnFullscreenModeChange(CefBrowser browser, bool fullscreen)
        {
            System.Diagnostics.Debug.WriteLine("Fullscreen: " + fullscreen);

            _dispatcher.Invoke(() =>
            {
                if (fullscreen)
                {
                    _currentWindow.WindowState = WindowState.FullScreen;
                }
                else
                {
                    _currentWindow.WindowState = WindowState.Normal;
                }
            });
        }
    }
}
