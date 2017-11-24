using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Utilities.Logger;

namespace Statistics.Survey.Analysis.Repository.Base
{
    public class BaseRepository
    {
        
        private static CustomDatabase _default = null;

        public static CustomDatabase DB
        {
            get { return _default ?? (_default = CustomDatabase.CreateDatabase("ConnStr")); }
        }

        private static CustomDatabase _surveyDB = null;
        public static CustomDatabase SurveyDB
        {
            get { return _surveyDB ?? (_surveyDB = CustomDatabase.CreateDatabase("ConnStrSurveyDB")); }
        }
        /// <summary>
        /// Disposes the command.
        /// </summary>
        /// <param name="command">The command.</param>
        protected static void DisposeCommand(MySqlCommand command)
        {
            if (command == null)
            {
                return;
            }

           /// LogEngine.Default.Debug("DisposeCommand", command);


            if (command.Connection != null)
            {
                if (command.Connection.State == ConnectionState.Open)
                {
                    command.Connection.Close();
                    command.Connection.Dispose();
                }
            }

            command.Dispose();
        }

        /// <summary>
        /// Disposes the command only.
        /// </summary>
        /// <param name="command">The command.</param>
        protected static void DisposeCommandOnly(MySqlCommand command)
        {
            if (command == null)
            {
                return;
            }

            LogEngine.Default.Debug("DisposeCommandOnly", command);

            command.Dispose();
        }

        /// <summary>
        /// Creates the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        protected static T[] CreateArray<T>(T obj, int length)
        {
            var objArray = new T[length];
            for (var i = 0; i < length; i++)
            {
                objArray[i] = obj;
            }
            return objArray;
        }
    }
}