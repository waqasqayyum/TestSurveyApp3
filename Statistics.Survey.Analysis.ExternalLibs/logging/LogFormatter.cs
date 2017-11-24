using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.Common;
using Utilities.Extensions;

namespace Utilities.Logger
{
	public class LogFormatter : ILogFormatter
	{
		/// <summary>
		/// Formats the object as string.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns></returns>
        private static string FormatTypeAsString(object obj)
        {
            StringBuilder sb = new StringBuilder(1024);

            if (obj is string || obj is Exception || obj is DbCommand || obj is DateTime || obj is TimeSpan || obj == null)
            {

            }
            else
            {
                sb.Append("[Type:" + obj.GetType().ToString() + "]");
            }

            if (obj == null)
            {
                sb.Append("Value: Null");
            }
            else if (obj is string)
            {
                sb.Append((string)obj);
            }
            else if (obj is DateTime)
            {
                sb.Append(((DateTime)obj).ToString("dd-MMM-yyyy hh:mm:ss tt"));
            }
            else if (obj is TimeSpan)
            {
                sb.Append(((TimeSpan)obj).FormatAsString());
            }
            else if (obj is DbCommand)
            {
                DbCommand cmd = (DbCommand)obj;
                string commandDetail = string.Empty;
                commandDetail = cmd.CommandType.ToString();

                /*if (cmd.Connection != null)
                {
                    commandDetail += "(" + cmd.Connection.i + ")";
                }*/

                sb.Append(" CommandText=[" + cmd.CommandText + "] CommandDetail=[" + commandDetail + "] Parameters= [");

                foreach (DbParameter param in cmd.Parameters)
                {
                    string paramValue = "null";
                    if (param.Value != null)
                    {
                        paramValue = param.Value.ToString();
                    }

                    sb.Append("@" + param.ParameterName + "=" + paramValue + " ");
                }
                sb.Append("]");
            }
            else if (obj is Exception)
            {
                sb.Append(" Exception=[" + obj.ToString().Replace("\r", "").Replace("\n", "\t") + "] Details=[" +
                ((Exception)obj).StackTrace.Replace("\r", "").Replace("\n", "\t") + "]");
            }
            else
            {
                var type = obj.GetType();

                foreach (var prop in type.GetProperties())
                {
                    var val = prop.GetValue(obj, new object[] { });

                    var valStr = string.Empty;
                    if (val != null)
                    {
                        if (prop.PropertyType == typeof(DateTime))
                        {
                            valStr = ((DateTime)val).ToString("dd-MMM-yyyy hh:mm:ss tt");
                        }
                        else if (prop.PropertyType == typeof(TimeSpan))
                        {
                            valStr = ((TimeSpan)val).FormatAsString();
                        }
                        else
                        {
                            valStr = val.ToString();
                        }
                        sb.Append("[" + prop.Name + ":" + valStr + "]");
                    }
                }
            }
            return sb.ToString();
        }

		 
		/// <summary>
		/// Formats the log.
		/// </summary>
		/// <param name="eventName">Name of the event.</param>
		/// <param name="details">The details.</param>
		/// <returns></returns>
        public string FormatLog(string eventName, object details, DateTime logDate)
        {
            int eventLength = 35;
            int methodLength = 55;
            int typeDetailLength = 77;
            List<string> eventList = WordWrap(eventName, eventLength);
            List<string> methodList = WordWrap(GetMethodNameFromStackTrace(), methodLength);
            List<string> typeDetailList = WordWrap(FormatTypeAsString(details), typeDetailLength);

            StringBuilder sb = new StringBuilder();

            int maxCount = eventList.Count;
            if (methodList.Count > maxCount)
            {
                maxCount = methodList.Count;
            }
            if (typeDetailList.Count > maxCount)
            {
                maxCount = typeDetailList.Count;
            }

            for (int i = 0; i < maxCount; i++)
            {
                sb.Append("\r\n");

                string line = string.Format("{0,-23} |{1,-" + eventLength + "} |{2,-" + methodLength + "} |{3,-" + typeDetailLength + "}|",
                    (i==0)?logDate.ToString("dd-MMM-yyy hh:mm:ss tt"):string.Empty,
                    GetValueFromList(eventLength, eventList, i),
                    GetValueFromList(methodLength, methodList, i),
                    GetValueFromList(typeDetailLength, typeDetailList, i));

                sb.Append(line);
            }

            return sb.ToString();
        }

        private static string GetValueFromList(int maxLength, List<string> list, int index)
        {
            string result = string.Empty;
            if (index < list.Count)
            {
                result= list[index];
            }
            else
            {
                result =new string(' ', maxLength);
            }
            return result;
        }


        public static List<string> WordWrap(string text, int width)
        {
            List<string> list = new List<string>();
            StringBuilder sb = new StringBuilder();

            if (text.Length < width)
            {
                list.Add(text);
            }
            else
            {

                for (int i = 0; i < text.Length; i++)
                {
                    if (i % width == 0 && i != 0)
                    {
                        list.Add(sb.ToString());
                        sb = new StringBuilder();
                    }

                    sb.Append(text[i]);
                }
                list.Add(sb.ToString());
            }
            return list;
        }

		/// <summary>
		/// Gets the name of the method from StackTrace.
		/// </summary>
		/// <returns></returns>
		private string GetMethodNameFromStackTrace()
		{
			try
			{
				var method = new StackFrame(4, true).GetMethod();
				string methodName = method.DeclaringType.ToString() + "." + method.Name + "()";
                methodName=methodName.Replace("TRACKER_REBORN_ACTIVITY", "");
				return methodName;
			}
			catch //(Exception ex)
			{
			}
			return string.Empty;
		}
	}
}
