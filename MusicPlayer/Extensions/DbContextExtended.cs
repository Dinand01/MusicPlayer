using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Data.Entity.SqlServerCompact;

namespace MusicPlayer.Extensions
{
    /// <summary>
    /// Custom cofiguration
    /// </summary>
    public class EntityConfiguration : DbConfiguration
    {
        /// <summary>
        /// Works magic making the config file absolete
        /// The startup method of the main form sets the default connection factory
        /// This registers the sql compact provider
        /// </summary>
        public EntityConfiguration()
        {
            SetProviderServices("System.Data.SqlServerCe.4.0", SqlCeProviderServices.Instance);

            SetExecutionStrategy("System.Data.SqlServerCe.4.0", () => new SqlAzureExecutionStrategy(10, new TimeSpan(0,0,0,1)));
            SetDefaultConnectionFactory(new LocalDbConnectionFactory("v4.0"));
        }

    }
}

