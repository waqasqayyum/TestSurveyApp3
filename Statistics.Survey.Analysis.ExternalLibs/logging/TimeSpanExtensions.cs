using System;

namespace Utilities.Extensions
{
	public static class TimeSpanExtensions
	{
		/// <summary>
		/// Formats as string.
		/// </summary>
		/// <param name="ts">The ts.</param>
		/// <returns></returns>
		public static string FormatAsString(this TimeSpan ts)
		{
			return ts.Days.ToString().PadLeft(2, '0') + ":" +
											ts.Hours.ToString().PadLeft(2, '0') + ":" +
											ts.Minutes.ToString().PadLeft(2, '0') + ":" +
											ts.Seconds.ToString().PadLeft(2, '0');
		}
	}
}