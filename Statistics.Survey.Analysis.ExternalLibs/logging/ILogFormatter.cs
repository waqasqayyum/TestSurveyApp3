using System;
namespace Utilities.Logger
{
	interface ILogFormatter
	{
        string FormatLog(string eventName, object details, DateTime dateTime);
    }
}
