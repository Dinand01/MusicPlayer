using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using System;
using Xilium.CefGlue.Avalonia;

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
            
            // Wait for browser to load, then set up JS interop
            _browser.LoadEnd += (sender, e) =>
            {
                // Only handle the main frame
                if (e.Frame.IsMain)
                {
                    Console.WriteLine("CefGlue browser loaded");
                    
                    // Set up JS interop after browser is ready
                    SetupJsInterop();
                }
            };
            
            // Add browser to the visual tree
            browserWrapper.Child = _browser;
            _musicPlayer = new MusicPlayerGate(_browser, this);
            
            // Set the URL using the Address property
            // Note: CefGlue normalizes URL to lowercase, so folder is "web" (lowercase)
            Console.WriteLine("Setting browser Address property...");
            _browser.Address = "local://web/Pages/index.html";
            
            this.KeyDown += MainWindow_KeyDown;
        }
        
        /// <summary>
        /// Set up JavaScript interop.
        /// </summary>
        private void SetupJsInterop()
        {
            // In CefGlue, inject JS object using ExecuteJavaScript
string script = @"
                window.MusicPlayer = {
                    togglePlay: function() { window.external && window.external.TogglePlay && window.external.TogglePlay(); },
                    nextSong: function() { window.external && window.external.NextSong && window.external.NextSong(); },
                    playSong: function(jsonSong) { window.external && window.external.Play && window.external.Play(jsonSong); },
                    toggleShuffle: function(shuffle) { window.external && window.external.Shuffle && window.external.Shuffle(shuffle); },
                    setVolume: function(volume) { window.external && window.external.SetVolume && window.external.SetVolume(volume); },
                    seekVideo: function(position) { window.external && window.external.SeekVideo && window.external.SeekVideo(position); },
                    moveToTime: function(seconds) { window.external && window.external.MoveToTime && window.external.MoveToTime(seconds); },
                    stop: function() { window.external && window.external.Stop && window.external.Stop(); },
                    hostServer: function(port) { window.external && window.external.HostServer && window.external.HostServer(port); },
                    connectToServer: function(ip, port) { window.external && window.external.ConnectToServer && window.external.ConnectToServer(ip, port); },
                    disconnectServer: function() { window.external && window.external.DisconnectServer && window.external.DisconnectServer(); },
                    startVideo: function(url) { window.external && window.external.StartVideo && window.external.StartVideo(url); },
                    stopVideo: function() { window.external && window.external.StopVideo && window.external.StopVideo(); },
                    copySongs: function(source, dest, number) { window.external && window.external.CopySongs && window.external.CopySongs(source, dest, number); },
                    openFolder: function() { window.external && window.external.OpenFolder && window.external.OpenFolder(); },
                    openFiles: function() { window.external && window.external.OpenFiles && window.external.OpenFiles(); },
                    getDefaultIP: function() { return new Promise(function(resolve, reject) { window.external && window.external.GetDefaultIP && window.external.GetDefaultIP(function(result) { resolve(result); }); }); },
                    getSongs: function(index, querry) { return new Promise(function(resolve, reject) { window.external && window.external.GetSongs && window.external.GetSongs(index, querry, function(result) { resolve(result); }); }); },
                    getCurrentSong: function() { return new Promise(function(resolve, reject) { window.external && window.external.GetCurrentSong && window.external.GetCurrentSong(function(result) { resolve(result); }); }); },
                    getShuffle: function() { return new Promise(function(resolve, reject) { window.external && window.external.GetShuffle && window.external.GetShuffle(function(result) { resolve(result); }); }); },
                    getVolume: function() { return new Promise(function(resolve, reject) { window.external && window.external.GetVolume && window.external.GetVolume(function(result) { resolve(result); }); }); },
                    getRadioStations: function(searchText) { return new Promise(function(resolve, reject) { window.external && window.external.GetRadioStations && window.external.GetRadioStations(searchText, function(result) { resolve(result); }); }); },
                    getRadioStation: function(id) { return new Promise(function(resolve, reject) { window.external && window.external.GetRadioStation && window.external.GetRadioStation(id, function(result) { resolve(result); }); }); }
                };
            ";
            _browser.ExecuteJavaScript(script, "musicplayer-inject", 0);
        }
        
        /// <summary>
        /// Handle keydown events for the main window.
        /// </summary>
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F5:
                    // Reload current page
                    _browser.ExecuteJavaScript("location.reload();", "musicplayer-reload", 0);
                    break;
                case Key.F12:
                    // Show dev tools
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
