using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace Statistics.Survey.Analysis.Repository.Base
{
    public class DALHelper
    {
        public delegate void MySqlDataReaderHandler(MySqlDataReader oDataReader);
        public delegate void MySqlDataReader_CommandHandler(MySqlDataReader oDataReader, MySqlCommand command);
        public delegate void MySqlDataReader_CommandHandlerr_Args(MySqlDataReader oDataReader, MySqlCommand command, ref object objArgument);

        private static string _connstr = null;
        public static string ConnStr
        {
            get
            {
                return _connstr;
            }
        }

        private DALHelper()
		{
			
		}

        static DALHelper()
		{
			_connstr = ConfigurationSettings.AppSettings["ConnStr"].ToString();
		}

        public static int ExecuteNonQuery(MySqlCommand cmd)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connstr))
                {
                    conn.Open();
                    cmd.Connection = conn;
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int ExecuteNonQuery(MySqlCommand cmd, String ConnectionString)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    cmd.Connection = conn;
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet FillDataSet(MySqlCommand cmd)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connstr))
                {
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                    cmd.Connection = conn;
                    DataSet dataSet = new DataSet();
                    dataAdapter.Fill(dataSet);
                    return dataSet;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable FillDataTable(MySqlCommand cmd)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connstr))
                {
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                    cmd.Connection = conn;
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static object ExecuteScalar(MySqlCommand cmd)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connstr))
                {
                    conn.Open();
                    cmd.Connection = conn;
                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ExecuteReader(MySqlCommand cmd, MySqlDataReaderHandler odrHandler)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connstr))
                {
                    conn.Open();
                    cmd.Connection = conn;

                    using (MySqlDataReader odr = cmd.ExecuteReader())
                    {
                        odrHandler(odr);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ExecuteReader(MySqlCommand cmd, MySqlDataReaderHandler odrHandler, String ConnectionString)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    cmd.Connection = conn;

                    using (MySqlDataReader odr = cmd.ExecuteReader())
                    {
                        odrHandler(odr);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ExecuteReader(MySqlCommand cmd, MySqlDataReader_CommandHandler oDataReader_CommmandHandler, String ConnectionString)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    cmd.Connection = conn;

                    using (MySqlDataReader odr = cmd.ExecuteReader())
                    {
                        oDataReader_CommmandHandler(odr, cmd);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public static void ExecuteReader(MySqlCommand cmd, MySqlDataReader_CommandHandler oDataReader_CommmandHandler)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connstr))
                {
                    conn.Open();
                    cmd.Connection = conn;

                    using (MySqlDataReader odr = cmd.ExecuteReader())
                    {
                        oDataReader_CommmandHandler(odr, cmd);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
                
        public static void ExecuteReader(MySqlCommand cmd, ref object objArgument, MySqlDataReader_CommandHandlerr_Args odrHandler, String ConnectionString)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    cmd.Connection = conn;

                    using (MySqlDataReader odr = cmd.ExecuteReader())
                    {
                        odrHandler(odr, cmd, ref objArgument);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ExecuteReader(MySqlCommand cmd, ref object objArgument, MySqlDataReader_CommandHandlerr_Args odrHandler)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connstr))
                {
                    conn.Open();
                    cmd.Connection = conn;

                    using (MySqlDataReader odr = cmd.ExecuteReader())
                    {
                        odrHandler(odr, cmd, ref objArgument);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ExecuteReader(MySqlCommand cmd, String ConnectionString)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    cmd.Connection = conn;

                    using (MySqlDataReader odr = cmd.ExecuteReader())
                    {
                        //nothing to do here for now
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ExecuteReader(MySqlCommand cmd)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connstr))
                {
                    conn.Open();
                    cmd.Connection = conn;

                    using (MySqlDataReader odr = cmd.ExecuteReader())
                    {
                        //nothing to do here for now
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool ExecuteReaderHasRows(MySqlCommand cmd)
        {
            bool result = false;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connstr))
                {
                    conn.Open();
                    cmd.Connection = conn;

                    using (MySqlDataReader odr = cmd.ExecuteReader())
                    {
                        result = odr.HasRows;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool ExecuteReaderHasRows(MySqlCommand cmd, String ConnectionString)
        {
            bool result = false;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    cmd.Connection = conn;

                    using (MySqlDataReader odr = cmd.ExecuteReader())
                    {
                        result = odr.HasRows;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static object ExecuteScalar(MySqlCommand cmd, String ConnectionString)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    cmd.Connection = conn;
                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable FillDataTable(MySqlCommand cmd, String ConnectionString)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                    cmd.Connection = conn;
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet FillDataSet(MySqlCommand cmd, String ConnectionString)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(ConnectionString))
                {
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(cmd);
                    cmd.Connection = conn;
                    DataSet dataSet = new DataSet();
                    dataAdapter.Fill(dataSet);
                    return dataSet;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}