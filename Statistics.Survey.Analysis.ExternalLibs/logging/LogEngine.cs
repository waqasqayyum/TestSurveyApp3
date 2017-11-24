using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Utilities.Logging.Enums;
using System.Web;


namespace Utilities.Logger
{
	public class LogEngine
	{
		private static object _lockObject = new object();
		private static LogEngine _logEngine = new LogEngine();
		private ILogFormatter _formatter;
		private ILogger _logger;

        private LogEngine()
        {

            _formatter = new LogFormatter();
            _logger = new Log4NetLogger();
        }

        //public static LogEngine Default
        //{
        //    get
        //    {
        //        lock (_lockObject)
        //        {
        //            if (_logEngine == null)
        //            {
        //                _logEngine = new LogEngine();
        //            }
        //        }
        //        return _logEngine;
        //    }
        //}


        public static LogEngine Default
        {
            get
            {
                if (_logEngine == null)
                {
                    lock (_lockObject)
                    {
                        if (_logEngine == null)
                            _logEngine = new LogEngine();

                    }
                }
                return _logEngine;
            }
        }

		/// <summary>
		/// Writes the log.
		/// </summary>
		/// <param name="logMode">The log mode.</param>
		/// <param name="eventName">Name of the event.</param>
		/// <param name="details">The details.</param>
        private void writeLog(LogMode logMode, string eventName, object details)
		{
            if (Convert.ToString(ConfigurationManager.AppSettings["Log.EnableLog"] ) == "1")
			{
				try
				{
					string logText = _formatter.FormatLog(eventName, details,DateTime.Now);
					if (logMode == LogMode.Debug)
					{
						_logger.Debug(logText);
					}
					else
					{
						_logger.Info(logText);
					}
				}
				catch (Exception ex)
				{
					WriteToEventLog("Error" + ex.ToString() + " " + ex.StackTrace);
				}
			}
		}

		/// <summary>
		/// Logs the specified event name.
		/// </summary>
		/// <param name="eventName">Name of the event.</param>
		/// <param name="message">The message.</param>
		public void Log(string eventName, object message)
		{
            writeLog(LogMode.Info, eventName, message);
		}
        /// <summary>
        /// Debugs the specified event name.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="message">The message.</param>
        public void Debug(string eventName, object message)
        {
            if (ConfigurationManager.AppSettings["Log.EnableDebugLog"] == "1")
            {
                writeLog(LogMode.Debug, eventName, message);
            }
        }

		/// <summary>
		/// Writes to event log.
		/// </summary>
		/// <param name="sEvent">The s event.</param>
		private void WriteToEventLog(string sEvent)
		{
			if (!EventLog.SourceExists("DMS_SERVICES"))
                EventLog.CreateEventSource("DMS_SERVICES", "Application");

            EventLog.WriteEntry("DMS_SERVICES", sEvent, EventLogEntryType.Error, 200);
		}

        public void log(string p, AggregateException ae)
        {
            throw new NotImplementedException();
        }
    }
}