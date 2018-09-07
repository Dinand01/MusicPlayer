using MusicPlayer.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicPlayer.Models;
using MusicPlayer;
using MusicPlayer.Interface;
using System.Threading;
using CefSharp.Wpf;
using CefSharp;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.Net;
using MusicPlayer.Controller;

namespace MusicPlayerWeb
{
    /// <summary>
    /// The music player UI class.
    /// </summary>
    /// <remarks>This partial contains the class variables and the methods for sending data to the UI from the backend.</remarks>
    public partial class MusicPlayerGate : IDisposable
    {
        /// <summary>
        /// The musicplayer instance.
        /// </summary>
        private IMusicPlayer _player;

        /// <summary>
        /// The instance for copying files.
        /// </summary>
        private ICopy _copy;
        
        /// <summary>
        /// The cromium web browser UI.
        /// </summary>
        private ChromiumWebBrowser _browser;

        /// <summary>
        /// The main dispatcher.
        /// </summary>
        private Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        /// <summary>
        /// The owner window.
        /// </summary>
        private Window _owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicPlayerGate" /> class.
        /// </summary>
        /// <param name="browser"></param>
        public MusicPlayerGate(ChromiumWebBrowser browser, Window window)
        {
            this._browser = browser;
            this._owner = window;
        }

        /// <summary>
        /// Dispose of the class.
        /// </summary>
        public void Dispose()
        {
            _browser?.Dispose();
            _browser = null;
            _player?.Dispose();
        }

        /// <summary>
        /// The copy progress has changed.
        /// </summary>
        /// <param name="percentage">The new percentage.</param>
        private void CopyProgressChanged(double percentage)
        {
            _browser?.ExecuteScriptAsync("window.CSSharpDispatcher.dispatchSetCopyProgress", percentage != 100 ? (double?)percentage : null);
        }

        /// <summary>
        /// The song has changed.
        /// </summary>
        /// <param name="song">The new song.</param>
        private void SongChanged(SongInformation song)
        {
            _browser?.ExecuteScriptAsync("window.CSSharpDispatcher.dispatchSetCurrentSong", JsonConvert.SerializeObject(song).Replace("\\", "\\\\"));
        }

        /// <summary>
        /// The server info changed.
        /// </summary>
        /// <param name="serverInfo">The new server info.</param>
        private void ServerInfoChanged(ServerInfo serverInfo)
        {
            _browser?.ExecuteScriptAsync("window.CSSharpDispatcher.dispatchSetServerInfo", JsonConvert.SerializeObject(serverInfo).Replace("\\", "\\\\"));
        }

        /// <summary>
        /// Creates a new player.
        /// </summary>
        /// <param name="player">The new player.</param>
        private void NewPlayer(IMusicPlayer player = null)
        {
            var server = (_player as IServer);
            if (server == null)
            {
                _player?.Dispose();
                _player = null;
                _player = player == null ? Factory.GetPlayer() : player;
                _player.SongChanged += SongChanged;
            }
        }
    }
}
