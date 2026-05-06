using Xilium.CefGlue;
using System.IO;
using System;

namespace MusicPlayerWeb
{
    /// <summary>
    /// Custom scheme handler factory for CefGlue.
    /// </summary>
    public class SchemeHandlerFactory : CefSchemeHandlerFactory
    {
        public const string SchemeName = "custom";
        private string _baseDirectory;

        public SchemeHandlerFactory(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }

        protected override CefResourceHandler Create(CefBrowser browser, CefFrame frame, string schemeName, CefRequest request)
        {
            // Parse the URL
            string url = request.Url;
            string path = url.Substring((SchemeName + "://").Length);
            
            // Security: prevent directory traversal
            path = path.Replace("..", "").Replace("//", "/");
            
            string fullPath = Path.Combine(_baseDirectory, path);
            
            if (File.Exists(fullPath))
            {
                return new FileResourceHandler(fullPath);
            }
            
            // Return 404 handler
            return new NotFoundResourceHandler();
        }
    }

    /// <summary>
    /// Custom resource handler for serving files.
    /// </summary>
    public class FileResourceHandler : CefResourceHandler
    {
        private readonly string _filePath;
        private FileStream _fileStream;
        private readonly string _mimeType;
        private long _fileSize;
        private bool _opened;

        public FileResourceHandler(string filePath)
        {
            _filePath = filePath;
            _mimeType = GetMimeType(Path.GetExtension(filePath));
        }

        protected override bool Open(CefRequest request, out bool handleRequest, CefCallback callback)
        {
            handleRequest = true;
            
            try
            {
                _fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                _fileSize = _fileStream.Length;
                _opened = true;
                return true;
            }
            catch
            {
                _opened = false;
                return false;
            }
        }

        protected override void GetResponseHeaders(CefResponse response, out long responseLength, out string redirectUrl)
        {
            responseLength = _fileSize;
            response.Status = 200;
            response.MimeType = _mimeType;
            response.StatusText = "OK";
            redirectUrl = null;
        }

        protected override bool Read(Stream responseStream, int bytesToRead, out int bytesRead, CefResourceReadCallback callback)
        {
            if (_fileStream == null || !_opened)
            {
                bytesRead = 0;
                return false;
            }

            byte[] buffer = new byte[bytesToRead];
            bytesRead = _fileStream.Read(buffer, 0, bytesToRead);
            if (bytesRead > 0)
            {
                responseStream.Write(buffer, 0, bytesRead);
            }
            return bytesRead > 0;
        }

        protected override bool Skip(long bytesToSkip, out long bytesSkipped, CefResourceSkipCallback callback)
        {
            bytesSkipped = 0;
            return false;
        }

        protected override void Cancel()
        {
            _fileStream?.Close();
            _fileStream = null;
            _opened = false;
        }

        private static string GetMimeType(string extension)
        {
            return extension.ToLower() switch
            {
                ".html" or ".htm" => "text/html",
                ".js" => "application/javascript",
                ".css" => "text/css",
                ".json" => "application/json",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".ico" => "image/x-icon",
                ".svg" => "image/svg+xml",
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".mp4" => "video/mp4",
                _ => "application/octet-stream"
            };
        }
    }

    /// <summary>
    /// Resource handler for 404 responses.
    /// </summary>
    public class NotFoundResourceHandler : CefResourceHandler
    {
        private readonly string _response = "404 Not Found";
        private System.IO.MemoryStream _stream;
        private bool _opened;

        protected override bool Open(CefRequest request, out bool handleRequest, CefCallback callback)
        {
            handleRequest = true;
            _stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(_response));
            _opened = true;
            return true;
        }

        protected override void GetResponseHeaders(CefResponse response, out long responseLength, out string redirectUrl)
        {
            responseLength = _stream.Length;
            response.Status = 404;
            response.MimeType = "text/plain";
            response.StatusText = "Not Found";
            redirectUrl = null;
        }

        protected override bool Read(Stream responseStream, int bytesToRead, out int bytesRead, CefResourceReadCallback callback)
        {
            if (_stream == null || !_opened)
            {
                bytesRead = 0;
                return false;
            }

            byte[] buffer = new byte[bytesToRead];
            bytesRead = _stream.Read(buffer, 0, bytesToRead);
            if (bytesRead > 0)
            {
                responseStream.Write(buffer, 0, bytesRead);
            }
            return bytesRead > 0;
        }

        protected override bool Skip(long bytesToSkip, out long bytesSkipped, CefResourceSkipCallback callback)
        {
            bytesSkipped = 0;
            return false;
        }

        protected override void Cancel()
        {
            _stream?.Close();
            _stream = null;
            _opened = false;
        }
    }
}
