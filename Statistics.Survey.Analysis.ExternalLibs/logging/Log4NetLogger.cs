using System;
using System.Configuration;
using System.IO;
using System.Reflection;

using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using Utilities.Logger.mylog4net;

namespace Utilities.Logger
{
    internal class Log4NetLogger : ILogger
    {
        private const string LOG_FILE_NAME = "Log";
        private ILog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetLogger"/> class.
        /// </summary>
        public Log4NetLogger()
        {
            _log = GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private static ILog GetLogger(Type type)
        {
            if (LogManager.GetCurrentLoggers().Length == 0)
            {
                ConfigureLog4Net();
            }
            return LogManager.GetLogger(type);
        }

/*        /// <summary>
        /// Configures the log4 net.
        /// </summary>
        private static void ConfigureLog4Net()
        {
            var Layout = new PatternLayout("%message");
            var Appender = new CompressedRollingFileAppender();

			string currentDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string logDirectoryPath = Path.Combine(currentDirectoryPath, "Logs");
			if (Directory.Exists(logDirectoryPath))
			{
				Directory.CreateDirectory(logDirectoryPath);
			}

			Appender.File = Path.Combine(logDirectoryPath, ServiceIdentity.LOG_FILE_NAME + ".log"); ;
            Appender.Layout = Layout;
            Appender.AppendToFile = true; // we will start a new one when the program starts'
            Appender.Name = "CompressedRollingFileAppender";
            Appender.Threshold = Level.All;
            Appender.RollingStyle = RollingFileAppender.RollingMode.Composite;//This means it will start a new log file each time the log grows to 10Mb'
            Appender.MaximumFileSize = "20MB";
            Appender.DatePattern = "yyyy-MM-dd";
            Appender.MaxSizeRollBackups = -1;// 'keep an infinite number of logs'
            Appender.StaticLogFileName = true;
            Appender.CountDirection = 1;// ' to reduce rollover costs'
            log4net.Config.BasicConfigurator.Configure(Appender);
            Appender.ActivateOptions();
        }
        */
        /// <summary>
        /// Configures the log4 net.
        /// </summary>
        private static void ConfigureLog4Net()
        {
            AddAppender("Info", Level.Info, Level.Info, Level.Info);
            AddAppender("Debug", Level.Debug, Level.Debug, Level.Debug);
        }

        private static void AddAppender(string directoryName, Level minLevel, Level maxLevel, Level threshold)
        {
            var layout = new PatternLayout("%message");
            var appender = new CompressedRollingFileAppender();

            //string currentDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string currentDirectoryPath = "C:\\DMS_LOGS\\"; // in case of Log Folder Path not configured
            try
            {
                currentDirectoryPath = ConfigurationManager.AppSettings["Log.LogFolder"].ToString();
            }
            catch (Exception)
            {
                
            }
            
            
            
            if (currentDirectoryPath != null)
            {
                var directory = Path.Combine(currentDirectoryPath, "Logs");

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                directory = Path.Combine(directory, directoryName);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                appender.File = Path.Combine(directory, LOG_FILE_NAME + ".log");
            }
            appender.Layout = layout;
            appender.AppendToFile = true; // we will start a new one when the program starts'
            appender.Name = "CompressedRollingFileAppender";
            appender.Threshold = Level.All;
            appender.RollingStyle = RollingFileAppender.RollingMode.Composite; //This means it will start a new log file each time the log grows to 20Mb'
            appender.MaximumFileSize = "20MB";
            appender.DatePattern = "yyyy-MM-dd";
            appender.MaxSizeRollBackups = 2000; // 'keep an infinite number of logs'
            appender.StaticLogFileName = false;
            appender.CountDirection = 1; // ' to reduce rollover costs'
            var filter = new LevelRangeFilter { LevelMin = minLevel, LevelMax = maxLevel };
            appender.AddFilter(filter);
            appender.Threshold = threshold;
            log4net.Config.BasicConfigurator.Configure(appender);
            appender.ActivateOptions();
        }


        #region ILogger Members

        /// <summary>
        /// Write the the specified message as debug.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            _log.Debug(message);
        }

        /// <summary>
        /// Write the specified message as information.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            _log.Info(message);
        }

        #endregion
    }
}
