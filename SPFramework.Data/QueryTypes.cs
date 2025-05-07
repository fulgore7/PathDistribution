namespace SPFramework.Data
{
    public class StoredProcedure : BaseQuery
    {
        public StoredProcedure(string storedProcName, string connectionString, ConnectionType connectType)
            : base(storedProcName, connectionString, connectType)
        {
        }
    }

    public class Query : BaseQuery
    {
        public Query(string sqlStatement, string connectionString, ConnectionType connectType)
            : base(sqlStatement, connectionString, connectType)
        {
        }
    }

    public class BulkCopy : BaseQuery
    {
        public BulkCopy(string tableName, string connectionString, ConnectionType connectType)
            : base(tableName, connectionString, connectType)
        {
        }
    }
}