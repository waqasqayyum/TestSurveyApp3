using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utilities.Logging.Enums
{
    internal enum RollPoint
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