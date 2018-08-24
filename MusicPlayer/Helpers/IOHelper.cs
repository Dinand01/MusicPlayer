using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Helpers
{
    /// <summary>
    /// Class with helper methods for IO related tasks.
    /// </summary>
    public static class IOHelper
    {
        /// <summary>
        /// Reads an embedded resource as text.
        /// </summary>
        /// <param name="resourceName">The resource name.</param>
        /// <returns>The content as text.</returns>
        public static string ReadEmbeddedResourceText(string resourceName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Could not read embedded resource: " + resourceName);
                return null;
            }
        }
    }
}
