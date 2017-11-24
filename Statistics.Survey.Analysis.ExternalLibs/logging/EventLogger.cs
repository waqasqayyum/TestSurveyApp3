using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;



namespace Utilities.Logger
{
    internal class EventLogger : ILogger
    {
        #region ILogger Members

        public void Debug(string message)
        {
            WriteToEventLog(message);
        }

        public void Info(string message)
        {
            WriteToEventLog(message);
        }


        /// <param name="sEvent">The s event.</param>
        private void WriteToEventLog(string sEvent)
        {
            if (!EventLog.SourceExists("DMS_LOGS"))
                EventLog.CreateEventSource("DMS_LOGS", "Application");

            EventLog.WriteEntry("DMS_LOGS", sEvent, EventLogEntryType.Error, 200);
        }
        #endregion
    }
}
