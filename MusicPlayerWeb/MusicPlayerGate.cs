using MusicPlayer.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer.Models;
using MusicPlayer;
using MusicPlayer.Interface;
using System.Threading;
using Xilium.CefGlue.Avalonia;
using Avalonia.Threading;
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
        /// The chromium web browser UI.
        /// </summary>
        private AvaloniaCefBrowser _browser;

        /// <summary>
        /// The main dispatcher.
        /// </summary>
        private Dispatcher _dispatcher = Dispatcher.UIThread;

        /// <summary>
        /// The owner window.
        /// </summary>
        private Avalonia.Controls.Window _owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicPlayerGate" /> class.
        /// </summary>
        /// <param name="browser"></param>
        public MusicPlayerGate(AvaloniaCefBrowser browser, Avalonia.Controls.Window window)
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
            string script = $"window.CSSharpDispatcher.dispatchSetCopyProgress({(percentage != 100 ? percentage.ToString() : "null")})";
            _browser?.ExecuteJavaScript(script, "musicplayer", 0);
        }

        /// <summary>
        /// The song has changed.
        /// </summary>
        /// <param name="song">The new song.</param>
        private void SongChanged(SongInformation song)
        {
            string json = JsonConvert.SerializeObject(song).Replace("\\", "\\\\");
            string script = $"window.CSSharpDispatcher.dispatchSetCurrentSong({json})";
            _browser?.ExecuteJavaScript(script, "musicplayer", 0);
        }

        /// <summary>
        /// The server info changed.
        /// </summary>
        /// <param name="serverInfo">The new server info.</param>
        private void ServerInfoChanged(ServerInfo serverInfo)
        {
            string json = JsonConvert.SerializeObject(serverInfo).Replace("\\", "\\\\");
            string script = $"window.CSSharpDispatcher.dispatchSetServerInfo({json})";
            _browser?.ExecuteJavaScript(script, "musicplayer", 0);
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
