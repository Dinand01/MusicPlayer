using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer
{
    /// <summary>
    /// The logger.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// The logging configuration.
        /// </summary>
        private static LoggingConfiguration _config;

        /// <summary>
        /// Gets a logger.
        /// </summary>
        private static NLog.Logger _logger
        {
            get
            {
                return LogManager.GetLogger("MusicPlayerLogger");
            }
        }

        /// <summary>
        /// Logs information.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogInfo(string message)
        {
            _config = _config == null ? CreateLogConfig() : _config;
            _logger.Log(LogLevel.Info, message);
        }

        /// <summary>
        /// Logs an error.
        /// </summary>
        /// <param name="e">The error to log.</param>
        /// <param name="message">The message to log.</param>
        public static void LogError(Exception e, string message = null)
        {
            _config = _config == null ? CreateLogConfig() : _config;
            _logger.Log(LogLevel.Error, e, message);
        }

        /// <summary>
        /// Creates a log config.
        /// </summary>
        /// <returns>The logging configuration.</returns>
        private static LoggingConfiguration CreateLogConfig()
        {
            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            fileTarget.FileName = "${basedir}/logs/logfile.txt";
            fileTarget.Layout = "${longdate} | ${level} | ${message} | ${exception}";

            var rule = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule);

            LogManager.Configuration = config;
            return config;
        }
    }
}
