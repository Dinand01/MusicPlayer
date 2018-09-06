using MusicPlayer.Controller;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    /// <summary>
    /// Describes a song.
    /// </summary> 
    /// <remarks>
    /// Properties with the datamember attribute are send over the WCF service.
    /// Properties with the jsonproperty atttribute are available in the UI.
    /// </remarks>
    [Serializable]
    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
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
            this.Location = path;;
            var temp = path.Split('\\').Last();
            var t2 = temp.Split('-');
            if (t2.Length > 1)
            {
                this.Title = t2.Last();
                this.Band = t2.First();
            }
            else
            {
                this.Title = t2.First();
            }
        }

        /// <summary>
        /// Initializes the song information from a radio station.
        /// </summary>
        /// <param name="radio">The internet radio station.</param>
        public SongInformation(RadioStation radio)
        {
            this.Location = radio.Url;
            this.Title = radio.Name;
            this.Genre = radio.Genre;
            this.IsInternetRadio = true;
            this.IsResolved = true;
        }

        [Key]
        [StringLength(255)]
        [JsonProperty]
        public string Location { get; set; }

        [JsonProperty]
        public DateTime? DateAdded { get; set; }

        [StringLength(512)]
        [DataMember]
        [JsonProperty]
        public string Title { get; set; }

        [StringLength(512)]
        [DataMember]
        public string FileName { get; set; }

        [StringLength(512)]
        [DataMember]
        [JsonProperty]
        public string Band { get; set; }

        [StringLength(512)]
        [DataMember]
        [JsonProperty]
        public string Album { get; set; }

        [StringLength(512)]
        [DataMember]
        [JsonProperty]
        public string Genre { get; set; }

        [DataMember]
        [JsonProperty]
        public DateTime? DateCreated { get; set; }

        [JsonProperty]
        public string SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the duration in seconds.
        /// </summary>
        [DataMember]
        [JsonProperty]
        public long Duration { get; set; }

        /// <summary>
        /// Gets or sets the current position of the song.
        /// </summary>
        [JsonProperty]
        public long Position { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        [DataMember]
        [JsonProperty]
        public byte[] Image { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the song is playing.
        /// </summary>
        [JsonProperty]
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
        [NotMapped]
        internal bool IsResolved { get; set; }
    }
}
