using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer.Models;
using MusicPlayer.Extensions;

namespace MusicPlayer.DAL
{
    /// <summary>
    /// The database context.
    /// </summary>
    [DbConfigurationType(typeof(EntityConfiguration))] 
    public class DbContext : System.Data.Entity.DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbContext" /> class.
        /// </summary>
        public DbContext()
        {
            Database.SetInitializer<DAL.DbContext>(new CreateDatabaseIfNotExists<DAL.DbContext>());
        }

        /// <summary>
        /// Gets or sets  Metadata for songs.
        /// </summary>
        public DbSet<Song> Songs { get; set; }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        public DbSet<Setting> Settings { get; set; }
    }
}
