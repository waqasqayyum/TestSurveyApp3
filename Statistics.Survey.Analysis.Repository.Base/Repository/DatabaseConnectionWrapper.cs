using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Data.Common;

namespace Statistics.Survey.Analysis.Repository.Base
{
    public class DatabaseConnectionWrapper : IDisposable
    {
        private int _refCount;

        /// <summary>
        /// Create a new <see cref="DatabaseConnectionWrapper"/> that wraps
        /// the given <paramref name="connection"/>.
        /// </summary>
        /// <param name="connection">Database connection to manage the lifetime of.</param>
        public DatabaseConnectionWrapper(MySqlConnection connection)
        {
            Connection = connection;
            _refCount = 1;
        }

        /// <summary>
        /// The underlying <see cref="DbConnection"/> we're managing.
        /// </summary>
        public MySqlConnection Connection { get; private set; }

        /// <summary>
        /// Has this wrapper disposed the underlying connection?
        /// </summary>
        public bool IsDisposed
        {
            get { return _refCount == 0; }
        }

        #region IDisposable Members

        /// <summary>
        /// Decrement the reference count and, if refcount is 0, close the underlying connection.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Usual Dispose pattern folderal to shut up FxCop.
        /// </summary>
        /// <param name="disposing">true if called via <see cref="DatabaseConnectionWrapper.Dispose()"/> method, false
        /// if called from finalizer. Of course, since we have no finalizer this will never
        /// be false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                int count = Interlocked.Decrement(ref _refCount);
                if (count == 0)
                {
                    Connection.Dispose();
                    Connection = null;
                    GC.SuppressFinalize(this);
                }
            }
        }

        #endregion

        /// <summary>
        /// Increment the reference count for the wrapped connection.
        /// </summary>
        public DatabaseConnectionWrapper AddRef()
        {
            Interlocked.Increment(ref _refCount);
            return this;
        }
    }
}