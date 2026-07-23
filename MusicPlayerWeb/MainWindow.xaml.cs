using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;
using Xilium.CefGlue.Avalonia;
using Xilium.CefGlue.Common;

namespace MusicPlayerWeb
{
    /// <summary>
    /// Interaction logic for MainWindow.axaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The CefGlue browser instance.
        /// </summary>
        private AvaloniaCefBrowser _browser;
        
        /// <summary>
        /// The instance of the musicplayer interface.
        /// </summary>
        private MusicPlayerGate _musicPlayer;

        public MainWindow()
        {
            InitializeComponent();
            
            // Get the browser wrapper from XAML
            var browserWrapper = this.FindControl<Decorator>("BrowserWrapper");
            
            // Create browser using AvaloniaCefBrowser from CefGlue.Avalonia
            // CEF is already initialized in Program.cs
            _browser = new AvaloniaCefBrowser();
            Console.WriteLine("AvaloniaCefBrowser created successfully");
            
            // Add browser to the visual tree
            browserWrapper.Child = _browser;
            _musicPlayer = new MusicPlayerGate(_browser, this);
            
            // Register MusicPlayerGate as a JavaScript object 
            _browser.Initialized += (sender, e) =>
            {
                // Clean this up - REGISTERJAVASCRIPTOBJECT REMOVED FOR CEFGLUE.CEPLPA
                // _browser.RegisterJavascriptObject(_musicPlayer, "MusicPlayer");
                // Instead, we register the music player using the proper CefGlue binding system
                _browser.RegisterJavascriptObject(_musicPlayer, "MusicPlayer");
            };
            
            // Set the URL using the Address property
            Console.WriteLine("Setting browser Address property...");
            _browser.Address = "local://web/Pages/index.html";
            
            this.KeyDown += MainWindow_KeyDown;
        }
        
        /// <summary>
        /// Handle keydown events for the main window.
        /// </summary>
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    _browser.ExecuteJavaScript("location.reload();", "musicplayer-reload", 0);
                    break;
                case Key.F12:
                    _browser.ShowDeveloperTools();
                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// Dispose of the music player UI.
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _browser?.UnregisterJavascriptObject("MusicPlayer");
            _musicPlayer?.Dispose();
            _musicPlayer = null;
        }
        
        /// <summary>
        /// Play or Pause the music.
        /// </summary>
        private void PlayPause_Click(object sender, EventArgs e)
        {
            _musicPlayer.TogglePlay();
        }
        
        /// <summary>
        /// Skip to the next song.
        /// </summary>
        private void Next_Click(object sender, EventArgs e)
        {
            _musicPlayer.NextSong();
        }
    }
}
