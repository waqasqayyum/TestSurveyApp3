using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Logger.mylog4net
{
	public enum RollPoint
	{
		HalfDay = 2,
		InvalidRollPoint = -1,
		TopOfDay = 3,
		TopOfHour = 1,
		TopOfMinute = 0,
		TopOfMonth = 5,
		TopOfWeek = 4
	}
}
