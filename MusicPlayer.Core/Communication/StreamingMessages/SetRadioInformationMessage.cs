using MusicPlayer.Core.Models;
using System;

namespace MusicPlayer.Core.Communication.StreamingMessages
{
    public class SetRadioInformationMessage : StreamingMessage
    {
        [Obsolete("For serialization only", true)]
        public SetRadioInformationMessage()
        {
        }

        public SetRadioInformationMessage(SongInformation song)
        {
            Location = song.Location;
        }

        public string Location { get; set; }
    }
}
