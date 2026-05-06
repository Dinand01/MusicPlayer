using MusicPlayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer
{
    /// <summary>
    /// The database context using EF Core.
    /// </summary>
    internal class Db : DbContext
    {
        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        public DbSet<Setting> Settings { get; set; }

        /// <summary>
        /// Gets or sets the radio stations.
        /// </summary>
        public DbSet<RadioStation> RadioStations { get; set; }

        /// <summary>
        /// Configures the database context.
        /// </summary>
        /// <param name="optionsBuilder">The options builder.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=MusicPlayer.db");
        }

        /// <summary>
        /// Create the database model.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Setting entity - key is Name (configured via [Key] attribute)
            modelBuilder.Entity<Setting>(entity =>
            {
                entity.HasKey(e => e.Name);
            });

            // Configure RadioStation entity - key is ID
            modelBuilder.Entity<RadioStation>(entity =>
            {
                entity.HasKey(e => e.ID);
            });
        }
    }
}
