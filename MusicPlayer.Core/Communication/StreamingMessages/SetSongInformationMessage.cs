using MusicPlayer.Core.Models;
using System;

namespace MusicPlayer.Core.Communication.StreamingMessages
{
    public class SetSongInformationMessage : StreamingMessage
    {
        [Obsolete("For serialization only", true)]
        public SetSongInformationMessage()
        {
        }

        public SetSongInformationMessage(SongInformation song)
        {
            Title = song.Title;
            Band = song.Band;
            Album = song.Album;
            Genre = song.Genre;
            DateCreated = song.DateCreated;
            Duration = song.Duration;
            Position = song.Position;
            Image = song.Image;
            ImageUrl = song.ImageUrl;
        }

        public string Title { get; set; }

        public string Band { get; set; }

        public string Album { get; set; }

        public string Genre { get; set; }

        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the duration in seconds.
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// Gets or sets the current position of the song.
        /// </summary>
        public long Position { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public byte[] Image { get; set; }

        /// <summary>
        /// Gets or sets the image url.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is an internet radio.
        /// </summary>
        public bool IsInternetRadio { get; set; }
    }
}
