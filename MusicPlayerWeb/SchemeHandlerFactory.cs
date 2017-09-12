using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerWeb
{
    /// <summary>
    /// Class to handle file requests from the browser.
    /// </summary>
    internal class SchemeHandlerFactory : ISchemeHandlerFactory
    {
        /// <summary>
        /// The name of the customer scheme.
        /// </summary>
        public const string SchemeName = "custom";

        /// <summary>
        /// The application root folder.
        /// </summary>
        private string _root;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemeHandlerFactory" /> class.
        /// </summary>
        /// <param name="root"></param>
        public SchemeHandlerFactory(string root)
        {
            _root = root;
        }

        /// <summary>
        /// Creates a resourceHandler for the request.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="frame"></param>
        /// <param name="schemeName"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            var uri = new Uri(request.Url);
            var fileName = uri.AbsolutePath;

            string resource = _root + "Web\\Scripts\\Build\\" + fileName.Replace('/', '\\');

            var fileExtension = Path.GetExtension(fileName);
            if ((new string[] { ".woff", ".woff2", ".ttf" }).Contains(fileExtension))
            {
                return ResourceHandler.FromFilePath(resource, "text/html");
            }

            return ResourceHandler.FromFilePath(resource, ResourceHandler.GetMimeType(fileExtension));
        }
    }
}
