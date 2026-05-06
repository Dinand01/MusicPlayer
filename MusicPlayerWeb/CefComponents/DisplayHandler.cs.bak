using CefSharp;
using CefSharp.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MusicPlayerWeb.CefComponents
{
    /// <summary>
    /// Custom displayhandler.
    /// </summary>
    internal class DisplayHandler : IDisplayHandler
    {
        /// <summary>
        /// The current window.
        /// </summary>
        private Window _currentWindow;

        /// <summary>
        /// The dispatcher for the ui thread.
        /// </summary>
        private Dispatcher _dispatcher;

        /// <summary>
        /// The previous window style.
        /// </summary>
        private WindowStyle _prevStyle;

        /// <summary>
        /// The previous window state.
        /// </summary>
        private WindowState _prevState;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayHandler" /> class.
        /// </summary>
        /// <param name="window">The parent window.</param>
        /// <param name="disp">The dispatcher for the ui thread.</param>
        public DisplayHandler(Window window, Dispatcher disp)
        {
            _currentWindow = window;
            _dispatcher = disp;
        }

        public void OnAddressChanged(IWebBrowser browserControl, AddressChangedEventArgs addressChangedArgs)
        {
        }

        public bool OnAutoResize(IWebBrowser browserControl, IBrowser browser, CefSharp.Structs.Size newSize)
        {
            return true;
        }

        public bool OnConsoleMessage(IWebBrowser browserControl, ConsoleMessageEventArgs consoleMessageArgs)
        {
            return true;
        }

        public void OnFaviconUrlChange(IWebBrowser browserControl, IBrowser browser, IList<string> urls)
        {
        }

        /// <summary>
        /// Enter or exit full screen mode.
        /// </summary>
        /// <param name="browserControl">The browser control.</param>
        /// <param name="browser">The browser.</param>
        /// <param name="fullscreen">A boolean indicating whether to enter or exit fullscreen.</param>
        public void OnFullscreenModeChange(IWebBrowser browserControl, IBrowser browser, bool fullscreen)
        {
            Debug.WriteLine("Fullscreen: " + fullscreen);

            _dispatcher.Invoke(() =>
            {
                if (fullscreen)
                {
                    _prevStyle = _currentWindow.WindowStyle;
                    _currentWindow.WindowStyle = WindowStyle.None;
                    _prevState = _currentWindow.WindowState;
                    _currentWindow.WindowState = WindowState.Maximized;
                }
                else
                {
                    _currentWindow.WindowState = _prevState;
                    _currentWindow.WindowStyle = _prevStyle;
                }
            });
        }

        public void OnStatusMessage(IWebBrowser browserControl, StatusMessageEventArgs statusMessageArgs)
        {
        }

        public void OnTitleChanged(IWebBrowser browserControl, TitleChangedEventArgs titleChangedArgs)
        {
        }

        public bool OnTooltipChanged(IWebBrowser browserControl, string text)
        {
            return true;
        }

        public bool OnTooltipChanged(IWebBrowser browserControl, ref string text)
        {
            return true;
        }
    }
}
