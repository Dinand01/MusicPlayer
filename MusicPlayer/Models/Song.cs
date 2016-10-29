using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Models
{
    /// <summary>
    /// Describes a song
    /// </summary> 
    [SerializableAttribute]
    public class Song
    {
        public Song()
        {
            this.Location = null;
            this.Album = null;
            this.Band = null;
            this.DateAdded = null;
            this.DateCreated = null;
            this.Gengre = null;
            this.SearchTerm = null;
            this.Title = null;
        }

        public Song(string path)
        {
            this.Location = path;
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

        [Key]
        [StringLength(255)]
        public string Location { get; set; }

        public DateTime? DateAdded { get; set; }

        [StringLength(512)]
        public string Title { get; set; }

        [StringLength(512)]
        public string Band { get; set; }

        [StringLength(512)]
        public string Album { get; set; }

        [StringLength(512)]
        public string Gengre { get; set; }

        public DateTime? DateCreated { get; set; }

        public string SearchTerm { get; set; }

        public string[] ToSubItemArray()
        {
            var result = new string[4];
            result[0] = this.Band != null ? this.Band : string.Empty; 
            result[1] = this.Gengre != null ? this.Gengre : string.Empty; 
            result[2] = this.DateCreated != null ? this.DateCreated.ToString() : string.Empty; 
            result[3] = this.DateAdded != null ? this.DateAdded.ToString() : string.Empty; 

            return result;
        }
    }
}
