using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Transactions;

namespace Statistics.Survey.Analysis.Repository.Base
{
    public class CustomDatabase
    {
        internal string ConnectionString { get; set; }

        private CustomDatabase()
        {
            // constructor for Customdatabase
        }
        
        public static CustomDatabase CreateDatabase(string name)
        {
            var database = new CustomDatabase
                {
                    ConnectionString = ConfigurationManager.ConnectionStrings[name].ToString()
                };
            return database;
        }

        private MySqlConnection CreateConnection()
        {
            var newConnection = new MySqlConnection {ConnectionString = ConnectionString};
            return newConnection;
        }

        private static MySqlCommand CreateCommandByCommandType(CommandType commandType, string commandText)
        {
            var command = new MySqlCommand {CommandType = commandType, CommandText = commandText};

            return command;
        }

        private static void PrepareCommand(MySqlCommand command, MySqlConnection connection)
        {
            command.Connection = connection;
        }

        private static void PrepareCommand(MySqlCommand command, MySqlTransaction transaction)
        {
            PrepareCommand(command, transaction.Connection);
            command.Transaction = transaction;
        }

        private static void RollbackTransaction(MySqlTransaction tran)
        {
            tran.Rollback();
        }

        private static void CommitTransaction(MySqlTransaction tran)
        {
            tran.Commit();
        }

        private static MySqlTransaction BeginTransaction(MySqlConnection connection)
        {
            var tran = connection.BeginTransaction();
            return tran;
        }

        public MySqlCommand GetStoredProcCommand(string commandText)
        {
            var command = CreateCommandByCommandType(CommandType.StoredProcedure, commandText);
            return command;
        }

        public MySqlCommand GetSqlStringCommand(string query)
        {
            return CreateCommandByCommandType(CommandType.Text, query);
        }

        public int ExecuteNonQuery(MySqlCommand command)
        {
            using (var wrapper = GetOpenConnection())
            {
                PrepareCommand(command, wrapper.Connection);
                return DoExecuteNonQuery(command);
            }
        }

        public int ExecuteNonQuery(MySqlCommand command, MySqlTransaction transaction)
        {
            PrepareCommand(command, transaction);
            return DoExecuteNonQuery(command);
        }

        public DataSet ExecuteDataSet(MySqlCommand command)
        {
            var dataSet = new DataSet {Locale = CultureInfo.InvariantCulture};
            LoadDataSet(command, dataSet, "Table");
            return dataSet;
        }

        public DataSet ExecuteDataSet(MySqlCommand command, MySqlTransaction transaction)
        {
            var dataSet = new DataSet {Locale = CultureInfo.InvariantCulture};
            LoadDataSet(command, dataSet, "Table", transaction);
            return dataSet;
        }

        public object ExecuteScalar(MySqlCommand command)
        {
            if (command == null) { throw new ArgumentNullException("command"); }
            using (var wrapper = GetOpenConnection())
            {
                PrepareCommand(command, wrapper.Connection);
                return DoExecuteScalar(command);
            }
        }

        public object ExecuteScalar(MySqlCommand command, MySqlTransaction transaction)
        {
            PrepareCommand(command, transaction);
            return DoExecuteScalar(command);
        }

        private static int DoExecuteNonQuery(MySqlCommand command)
        {
            var rowsAffected = command.ExecuteNonQuery();
            return rowsAffected;
        }

        private static object DoExecuteScalar(MySqlCommand command)
        {
            var returnValue = command.ExecuteScalar();
            return returnValue;
        }

        public void LoadDataSet(MySqlCommand command, DataSet dataSet, string tableName)
        {

            LoadDataSet(command, dataSet, new[] {tableName});
        }

        public void LoadDataSet(MySqlCommand command, DataSet dataSet, string tableName, MySqlTransaction transaction)
        {
            LoadDataSet(command, dataSet, new[] {tableName}, transaction);
        }

        public void LoadDataSet(MySqlCommand command, DataSet dataSet, string[] tableNames,
                                MySqlTransaction transaction)
        {
            PrepareCommand(command, transaction);
            DoLoadDataSet(command, dataSet, tableNames);
        }

        public void LoadDataSet(MySqlCommand command, DataSet dataSet, string[] tableNames)
        {
            using (var wrapper = GetOpenConnection())
            {
                PrepareCommand(command, wrapper.Connection);
                DoLoadDataSet(command, dataSet, tableNames);
            }
        }

        private static void DoLoadDataSet(MySqlCommand command, DataSet dataSet, string[] tableNames)
        {

            using (var adapter = GetDataAdapter())
            {
                adapter.SelectCommand = command;

                const string systemCreatedTableNameRoot = "Table";
                for (var i = 0; i < tableNames.Length; i++)
                {
                    var systemCreatedTableName = (i == 0) ? systemCreatedTableNameRoot : systemCreatedTableNameRoot + i;
                    adapter.TableMappings.Add(systemCreatedTableName, tableNames[i]);
                }

                adapter.Fill(dataSet);
            }
        }

        public int UpdateDataSet(DataSet dataSet, string tableName, MySqlCommand insertCommand,
                                 MySqlCommand updateCommand, MySqlCommand deleteCommand,
                                 int? updateBatchSize)
        {
            using (var wrapper = GetOpenConnection())
            {
                if (Transaction.Current == null)
                {
                    var trans = BeginTransaction(wrapper.Connection);
                    try
                    {
                        int rowsAffected = UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand,
                                                         trans, updateBatchSize);
                        CommitTransaction(trans);
                        return rowsAffected;
                    }
                    catch
                    {
                        RollbackTransaction(trans);
                        throw;
                    }
                }

                if (insertCommand != null)
                {
                    PrepareCommand(insertCommand, wrapper.Connection);
                }
                if (updateCommand != null)
                {
                    PrepareCommand(updateCommand, wrapper.Connection);
                }
                if (deleteCommand != null)
                {
                    PrepareCommand(deleteCommand, wrapper.Connection);
                }

                return DoUpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, updateBatchSize);
            }
        }

        public int UpdateDataSet(DataSet dataSet, string tableName, MySqlCommand insertCommand,
                                 MySqlCommand updateCommand, MySqlCommand deleteCommand,
                                 MySqlTransaction transaction, int? updateBatchSize)
        {
            if (insertCommand != null)
            {
                PrepareCommand(insertCommand, transaction);
            }
            if (updateCommand != null)
            {
                PrepareCommand(updateCommand, transaction);
            }
            if (deleteCommand != null)
            {
                PrepareCommand(deleteCommand, transaction);
            }

            return DoUpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, updateBatchSize);
        }

        private static int DoUpdateDataSet(DataSet dataSet, string tableName, MySqlCommand insertCommand,
                                           MySqlCommand updateCommand, MySqlCommand deleteCommand,
                                           int? updateBatchSize)
        {
            using (var adapter = GetDataAdapter())
            {
                var explicitAdapter = adapter;
                if (insertCommand != null)
                {
                    explicitAdapter.InsertCommand = insertCommand;
                }
                if (updateCommand != null)
                {
                    explicitAdapter.UpdateCommand = updateCommand;
                }
                if (deleteCommand != null)
                {
                    explicitAdapter.DeleteCommand = deleteCommand;
                }

                if (updateBatchSize != null)
                {
                    adapter.UpdateBatchSize = (int) updateBatchSize;
                    if (insertCommand != null)
                    {
                        adapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;
                    }
                    if (updateCommand != null)
                    {
                        adapter.UpdateCommand.UpdatedRowSource = UpdateRowSource.None;
                    }
                    if (deleteCommand != null)
                    {
                        adapter.DeleteCommand.UpdatedRowSource = UpdateRowSource.None;
                    }
                }

                int rows = adapter.Update(dataSet.Tables[tableName]);
                return rows;
            }
        }

        private static MySqlDataAdapter GetDataAdapter()
        {
            var adapter = new MySqlDataAdapter();
            return adapter;
        }

        internal MySqlConnection GetNewOpenConnection()
        {
            MySqlConnection connection = null;
            try
            {
                connection = CreateConnection();
                connection.Open();
            }
            catch
            {
                if (connection != null)
                {
                    connection.Close();
                }

                throw;
            }

            return connection;
        }

        private DatabaseConnectionWrapper GetOpenConnection()
        {
            var connection = TransactionScopeConnections.GetConnection(this);
            return connection ?? GetWrappedConnection();
        }

        private DatabaseConnectionWrapper GetWrappedConnection()
        {
            return new DatabaseConnectionWrapper(GetNewOpenConnection());
        }

        public virtual IDataReader ExecuteReader(MySqlCommand command)
        {
            using (var wrapper = GetOpenConnection())
            {
                PrepareCommand(command, wrapper.Connection);
                var realReader = DoExecuteReader(command, CommandBehavior.Default);
                return CreateWrappedReader(wrapper, realReader);
            }
        }

        private IDataReader DoExecuteReader(MySqlCommand command,
                                                 CommandBehavior cmdBehavior)
        {
            var reader = command.ExecuteReader(cmdBehavior);
            return reader;
        }

        protected virtual IDataReader CreateWrappedReader(DatabaseConnectionWrapper connection, IDataReader innerReader)
        {
            return new RefCountingDataReader(connection, innerReader);
        }
    }
}