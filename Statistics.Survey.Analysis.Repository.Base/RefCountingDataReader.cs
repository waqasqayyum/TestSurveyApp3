using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Statistics.Survey.Analysis.Repository.Base
{
    public class RefCountingDataReader : DataReaderWrapper
    {
        private readonly DatabaseConnectionWrapper _connectionWrapper;

        /// <summary>
        /// Create a new <see cref='RefCountingDataReader'/> that wraps
        /// the given <paramref name="innerReader"/> and properly
        /// cleans the refcount on the given <paramref name="connection"/>
        /// when done.
        /// </summary>
        /// <param name="connection">Connection to close.</param>
        /// <param name="innerReader">Reader to do the actual work.</param>
        public RefCountingDataReader(DatabaseConnectionWrapper connection, IDataReader innerReader)
            : base(innerReader)
        {
            _connectionWrapper = connection;
            _connectionWrapper.AddRef();
        }

        /// <summary>
        /// Closes the <see cref="T:System.Data.IDataReader"/> Object.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Close()
        {
            if (!IsClosed)
            {
                base.Close();
                _connectionWrapper.Dispose();
            }
        }

        /// <summary>
        /// Clean up resources.
        /// </summary>
        /// <param name="disposing">True if called from dispose, false if called from finalizer. We have no finalizer,
        /// so this will never be false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!IsClosed)
                {
                    base.Dispose(true);
                    _connectionWrapper.Dispose();
                }
            }
        }
    }
}