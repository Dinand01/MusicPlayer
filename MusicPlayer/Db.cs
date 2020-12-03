using Microsoft.EntityFrameworkCore;
using MusicPlayer.Models;

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
        public Db(DbContextOptions<Db> options) : base(options)
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TODO: fix
            //Database.SetInitializer<Db>(new SqliteDropCreateDatabaseWhenModelChanges<Db>(modelBuilder));
            base.OnModelCreating(modelBuilder);
        }
    }
}
