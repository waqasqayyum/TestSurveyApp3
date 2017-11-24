using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Utilities.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Lefts the specified string value.
        /// </summary>
        /// <param name="StringValue">The string value.</param>
        /// <param name="Length">The length.</param>
        /// <returns></returns>
        public static string Left(this string StringValue, int Length)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(StringValue) || Length < 1)
                result = string.Empty;
            else
                result = StringValue.Substring(0, Math.Min(Length, StringValue.Length));
            return result;
        }

        /// <summary>
        /// Lefts the with dots.
        /// </summary>
        /// <param name="StringValue">The string value.</param>
        /// <param name="Length">The length.</param>
        /// <returns></returns>
        public static string LeftWithDots(this string StringValue, int Length)
        {

            var result = StringValue.Left(Length);
            if (result.Length < StringValue.Length)
            {
                result = result + " ...";

            }
            return result;
        }

        /// <summary>
        /// Gets the date.
        /// </summary>
        /// <param name="stringValue">The string value.</param>
        /// <returns></returns>
        public static DateTime? GetNullableDate(this string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                return null;
            }
            DateTime result;
            if (DateTime.TryParse(stringValue, out result))
                return (DateTime?)result;
            return null;
        }


        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="stringValue">The string value.</param>
        /// <returns></returns>
        public static string GetString(this string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
                return null;
            return stringValue;
        }

        /// <summary>
        /// Gets the nullable int.
        /// </summary>
        /// <param name="stringValue">The string value.</param>
        /// <returns></returns>
        public static int? GetNullableInt(this string stringValue)
        {
            int result;
            if (int.TryParse(stringValue, out result))
                return (int?)result;
            return null;
        }

        /// <summary>
        /// Gets the NVL int.
        /// </summary>
        /// <param name="stringValue">The string value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static int GetNvlInt(this string stringValue, int defaultValue)
        {
            int result;
            if (int.TryParse(stringValue, out result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// Gets the int.
        /// </summary>
        /// <param name="stringValue">The string value.</param>
        /// <returns></returns>
        public static int GetInt(this string stringValue)
        {
            return Convert.ToInt32(stringValue);
        }

        /// <summary>
        /// Gets the nullable bool.
        /// </summary>
        /// <param name="stringValue">The string value.</param>
        /// <returns></returns>
        public static bool? GetNullableBool(this string stringValue)
        {
            bool? result = null;

            if (string.IsNullOrEmpty(stringValue))
                result = null;
            else if (stringValue == "1" || stringValue.ToLower() == "true")
                result = true;
            else
                result = false;
            return result;
        }

        /// <summary>
        /// Gets the bool.
        /// </summary>
        /// <param name="stringValue">The string value.</param>
        /// <returns></returns>
        public static bool GetBool(this string stringValue)
        {
            bool result = false;

            if (string.IsNullOrEmpty(stringValue))
                result = false;
            else if (stringValue == "1" || stringValue.ToLower() == "true")
                result = true;
            else
                result = false;
            return result;
        }

        /// <summary>
        /// Removes the double space.
        /// </summary>
        /// <param name="stringValue">The string value.</param>
        /// <returns></returns>
        public static string RemoveDoubleSpace(this string stringValue)
        {
            while (stringValue.Contains("  "))
            {
                stringValue = stringValue.Replace("  ", " ");
            }
            return stringValue;
        }

        /// <summary>
        /// Toes the list from CSV.
        /// </summary>
        /// <param name="stringValue">The string value.</param>
        /// <returns></returns>
        public static List<int> ToListFromCSV(this string stringValue)
        {
            List<int> list = new List<int>();
            string[] stringList = stringValue.Split(',');

            foreach (var item in stringList)
            {
                list.Add(item.GetInt());
            }
            return list;

        }

        /// <summary>
        /// Gets the nullable double.
        /// </summary>
        /// <param name="stringValue">The string value.</param>
        /// <returns></returns>
        public static double? GetNullableDouble(this string stringValue)
        {
            double result;
            if (double.TryParse(stringValue, out result))
                return (double?)result;
            return null;
        }

        /// <summary>
        /// Gets the double.
        /// </summary>
        /// <param name="stringValue">The string value.</param>
        /// <returns></returns>
        public static double GetDouble(this string stringValue)
        {
            return Convert.ToDouble(stringValue);
        }

        /// <summary>
        /// Formats the double.
        /// </summary>
        /// <param name="stringValue">The string value.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string FormatDouble(this string stringValue, string format)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(stringValue))
            {
                var doubleVal = stringValue.GetNullableDouble();
                if (!doubleVal.HasValue)
                {
                    doubleVal = 0.0;
                }

                result = doubleVal.Value.ToString(format);
            }
            return result;
        }

        public static bool IsBlank(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsValidNumber(this string str)
        {
            int valid;
            var isNumeric = int.TryParse(str, out valid);
            if (isNumeric)
            {
                return true;
            }
            return false;
        }

        public static bool IsValidDecimal(this string str)
        {
            decimal valid;
            var isDecimal = decimal.TryParse(str, out valid);
            if (isDecimal)
            {
                return true;
            }
            return false;
        }


        public static bool IsValidMonth(this string str)
        {
            var months = "JAN,FEB,MAR,APR,MAY,JUN,JUL,AUG,SEP,OCT,NOV,DEC".Split(",".ToCharArray());
            str = str.ToUpper();
            if (months.Contains(str))
            {
                return true;
            }
            return false;
        }

        public static bool IsValidDateOrBlankString(this string str, string format)
        {
            if (string.IsNullOrEmpty(str))
                return true;

            string blankFormat = format.ToLower().Replace('d', '_').Replace('m', '_').Replace('y', '_');
            if (str == blankFormat)
            {
                return true;
            }

            DateTime valid;
            bool isDate = DateTime.TryParseExact(str,
                                                 format,
                                                 CultureInfo.InvariantCulture, DateTimeStyles.None, out valid);
            return isDate;
        }

        public static bool IsValidDate(this string str, string format)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            DateTime valid;
            bool isDate = DateTime.TryParseExact(str,
                                                 format,
                                                 CultureInfo.InvariantCulture, DateTimeStyles.None, out valid);
            return isDate;
        }

        

        public static bool IsValidDate(this string str)
        {
            DateTime valid;
            var format = new[] { "d-MMM-yy" };
            bool isDate = DateTime.TryParseExact(str,
                                                 format,
                                                 CultureInfo.InvariantCulture, DateTimeStyles.None, out valid);
            return isDate;
        }

        public static int GetNumber(this string str)
        {
            int valid;
            int.TryParse(str, out valid);
            return valid;
        }

        public static decimal GetDecimal(this string str)
        {
            decimal valid;
            decimal.TryParse(str, out valid);
            return valid;
        }

        public static DateTime GetDate(this string str)
        {
            DateTime valid;
            DateTime.TryParseExact(str,
                                   "d-MMM-yy",
                                   CultureInfo.InvariantCulture, DateTimeStyles.None, out valid);

            return valid;
        }

        public static DateTime GetDate(this string str,string format)
        {
            DateTime valid;
            DateTime.TryParseExact(str,
                                   format,
                                   CultureInfo.InvariantCulture, DateTimeStyles.None, out valid);

            return valid;
        }

        public static bool IsValidMonthComplete(this string str)
        {
            DateTime dateTime;
            var format = new[] { "yyyyMM" };
            bool isValid = DateTime.TryParseExact(str,
                                                  format,
                                                  CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
            if (isValid)
            {
                return true;
            }
            return false;
        }

        public static string GetNumberForMonth(this string strR)
        {
            var months = "JAN,FEB,MAR,APR,MAY,JUN,JUL,AUG,SEP,OCT,NOV,DEC".Split(",".ToCharArray());
            var monthNumber = "01,02,03,04,05,06,07,08,09,10,11,12".Split(",".ToCharArray());

            strR = strR.ToUpper();
            var result = string.Empty;
            for (var i = 0; i < months.Length; i++)
            {
                if (months[i].Equals(strR, StringComparison.CurrentCulture))
                {
                    result = monthNumber[i];
                }
            }

            return result;
        }

       

        public static string GetDateWithoutTime(this string str)
        {
            if (str.Length > 8)
            {
                return str.Remove(9);
            }
            return str;
        }
        public static string HandelSpecialCharacter(string str)
        {
            StringBuilder result=new StringBuilder();
            foreach(char ch in str)
            {
                if(ch.Equals('#'))
                    result.Append("%23");
                else if(ch.Equals('&'))
                    result.Append("%26");
                else if(ch.Equals('?'))
                    result.Append("%3F");
                else if(ch.Equals('@'))
                    result.Append("%40");
                else if(ch.Equals('`'))
                    result.Append("%60");
                else
                    result.Append(ch);
                
            }
            return result.ToString();
        }
        

    }

}