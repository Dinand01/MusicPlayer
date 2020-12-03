using System;
using System.Linq;

namespace MusicPlayer.Core.Models
{
    /// <summary>
    /// Describes a song.
    /// </summary> 
    /// <remarks>
    /// Properties with the datamember attribute are send over the WCF service.
    /// Properties with the jsonproperty atttribute are available in the UI.
    /// </remarks>
    public class SongInformation
    {
        /// <summary>
        /// Creates a new empty song.
        /// </summary>
        public SongInformation() { }

        /// <summary>
        /// Creates a song from a file path.
        /// </summary>
        /// <param name="path">The file path.</param>
        public SongInformation(string path)
        {
            Location = path; ;
            var temp = path.Split('\\').Last();
            var t2 = temp.Split('-');
            if (t2.Length > 1)
            {
                Title = t2.Last();
                Band = t2.First();
            }
            else
            {
                Title = t2.First();
            }
        }

        /// <summary>
        /// Initializes the song information from a radio station.
        /// </summary>
        /// <param name="radio">The internet radio station.</param>
        public SongInformation(RadioStation radio)
        {
            Location = radio.Url;
            Title = radio.Name;
            Genre = radio.Genre;
            ImageUrl = radio.ImageUrl;
            IsInternetRadio = true;
            IsResolved = true;
            Position = 0;
        }

        public string Location { get; set; }

        public DateTime? DateAdded { get; set; }

        public string Title { get; set; }

        public string FileName { get; set; }

        public string Band { get; set; }

        public string Album { get; set; }

        public string Genre { get; set; }

        public DateTime? DateCreated { get; set; }

        public string SearchTerm { get; set; }

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
        /// Gets or sets a value indicating whether the song is playing.
        /// </summary>
        public bool IsPlaying { get; set; }

        /// <summary>
        /// Gets or sets the file stream.
        /// </summary>
        public byte[] File { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is an internet radio.
        /// </summary>
        public bool IsInternetRadio { get; set; }

        /// <summary>
        /// Gets or sets a boolean indicating whether the song is resolved.
        /// </summary>
        internal bool IsResolved { get; set; }

        /// <summary>
        /// Gets a value indicating whether the song is stored on the internet.
        /// </summary>
        internal bool IsInternetLocation
        {
            get
            {
                return Uri.IsWellFormedUriString(Location, UriKind.Absolute);
            }
        }
    }
}
