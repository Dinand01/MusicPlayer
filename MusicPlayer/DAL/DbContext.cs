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
    /// The database context
    /// </summary>
    [DbConfigurationType(typeof(EntityConfiguration))] 
    public class DbContext : System.Data.Entity.DbContext
    {
        public DbSet<Song> Songs { get; set; }
    }
}
