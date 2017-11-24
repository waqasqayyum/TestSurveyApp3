using System;

namespace Utilities.Logger
{
	interface ILogger
	{
		void Debug(string message);
		void Info(string message);
	}
}
