using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    /// <summary>
    /// Enumeration of UI elements that are of intersest.
    /// </summary>
    public enum UIElements
    {
        /// <summary>
        /// Scrollable contianing the Dynamic lyout.
        /// </summary>
        MusicList,

        /// <summary>
        /// The home button.
        /// </summary>
        HomeButton,

        /// <summary>
        /// The audio button.
        /// </summary>
        AudioButton,

        /// <summary>
        /// The button to access the server page.
        /// </summary>
        ServerButton,

        /// <summary>
        /// The client button.
        /// </summary>
        ClientButton,

        /// <summary>
        /// The copy button.
        /// </summary>
        CopyButton,

        /// <summary>
        /// The top notification (visible on all pages).
        /// </summary>
        Notification,

        /// <summary>
        /// The play pause button.
        /// </summary>
        PlayPauseButton,

        /// <summary>
        /// Go to the next song.
        /// </summary>
        NextButton,

        /// <summary>
        /// The slider indicating the song position.
        /// </summary>
        Slider,

        /// <summary>
        /// The label that displays the current song.
        /// </summary>
        CurrentSong,

        /// <summary>
        /// The entered ip port.
        /// </summary>
        IPPort,

        /// <summary>
        /// The ip adress textbox.
        /// </summary>
        IPAddress,

        /// <summary>
        /// The button used to connect or disconnect a server.
        /// </summary>
        ServerStatusButton,

        /// <summary>
        /// The client status button.
        /// </summary>
        ClientStatusButton,

        /// <summary>
        /// The source folder to copy files from.
        /// </summary>
        CopyAmount,

        /// <summary>
        /// The copy progress bar.
        /// </summary>
        CopyProgress
    }
}
