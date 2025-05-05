using System;
using System.Data.SqlClient;

namespace SPFramework.Data
{
    public class SPTransaction : IDisposable
    {
        /// <summary>
        /// Gets the current transaction object
        /// </summary>
        ///
        public SqlTransaction Transaction
        {
            get;
        }

        public SPTransaction(SqlTransaction trans)
        {
            Transaction = trans;
        }

        /// <summary>
        /// Commits the transaction to the database and closes the connection
        /// </summary>
        ///
        public void Commit()
        {
            Transaction.Commit();
            Transaction.Connection.Close();
        }

        /// <summary>
        /// Rolls a bad transaction back and closes the connection
        /// </summary>
        ///
        public void Rollback()
        {
            Transaction.Rollback();
            Transaction.Connection.Close();
        }

        /// <summary>
        /// This method allows this class to wrap other calls in a using statement
        /// </summary>
        ///
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) Transaction.Dispose();
        }
    }
}