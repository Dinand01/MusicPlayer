using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.DAL
{
    /// <summary>
    /// A db context store, because we are working with a single file we do not want multiple threads reading and writing to the database (file) at the same time.
    /// This class serves to prevent this.
    /// </summary>
    public class DbContextStore : IDisposable
    {
        /// <summary>
        /// The instance.
        /// </summary>
        private static DbContextStore _instance = new DbContextStore();

        /// <summary>
        /// The access lock.
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// The access lock.
        /// </summary>
        private static readonly object _lock2 = new object();

        /// <summary>
        /// The database context.
        /// </summary>
        private DbContext _db;

        /// <summary>
        /// Prevents a default instance of the <see cref="DbContextStore" /> class.
        /// </summary>
        private DbContextStore()
        {
            this._db = new DbContext();

            try
            {
                this._db.Songs.Count();
            }
            catch
            {
                string location = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                location = (location.EndsWith("\\") ? location : location + "\\");
                File.Delete(location + "MusicPlayer.DAL.DbContext.sdf");
                this._db = new DbContext();
            }
        }

        /// <summary>
        /// Gets the db context.
        /// </summary>
        public DbContext Context
        {
            get
            {
                return this._db;
            }
        }

        /// <summary>
        /// Invalidate the corrent context.
        /// </summary>
        public void Invalidate()
        {
            this._db.Dispose();
            this._db = new DbContext();
        }

        /// <summary>
        /// Saves the changes to the database.
        /// </summary>
        public void SaveChanges()
        {
            lock (_lock2)
            {
                this._db.SaveChanges();
            }
        }

        /// <summary>
        /// Dispose of the controller.
        /// </summary>
        public void Dispose()
        {
            this._db.Dispose();
        }

        /// <summary>
        /// Gets the instance of this class.
        /// </summary>
        public static DbContextStore Ctrl
        {
            get
            {
                lock (_lock)
                {
                    return _instance;
                }
            }
        }
    }
}
