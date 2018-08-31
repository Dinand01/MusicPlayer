using MusicPlayer.Models;
using SQLite.CodeFirst;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite.EF6;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer
{
    /// <summary>
    /// The database context.
    /// </summary>
    internal class Db : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Db" /> class.
        /// </summary>
        public Db() : base("Db")
        {
        }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        public DbSet<Setting> Settings { get; set; }

        /// <summary>
        /// Gets or sets the radio stations.
        /// </summary>
        public DbSet<RadioStation> RadioStations { get; set; }

        /// <summary>
        /// Create the database.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<Db>(new SqliteDropCreateDatabaseWhenModelChanges<Db>(modelBuilder));
            base.OnModelCreating(modelBuilder);
        }
    }
}
