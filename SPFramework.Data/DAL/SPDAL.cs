namespace SPFramework.Data.DAL
{
    public class SPDAL
    {
        internal string ConnectionString { get; set; }
        internal ConnectionType ConnectType { get; set; }

        public SPDAL(string configName) : this(configName, ConnectionType.UseSecurityLoader)
        {
        }

        public SPDAL(string connectionString, ConnectionType connectType)
        {
            ConnectionString = connectionString;
            ConnectType = connectType;
        }

        public QueryBuilder<StoredProcedure> StoredProcedure(string storedProcName)
        {
            return new QueryBuilder<StoredProcedure>(new StoredProcedure(storedProcName, ConnectionString, ConnectType));
        }

        public QueryBuilder<Query> Query(string sqlStatement)
        {
            return new QueryBuilder<Query>(new Query(sqlStatement, ConnectionString, ConnectType));
        }

        public QueryBuilder<BulkCopy> BulkInsert(string tableName)
        {
            return new QueryBuilder<BulkCopy>(new BulkCopy(tableName, ConnectionString, ConnectType));
        }

        public SPTransaction CreateTransaction()
        {
            return new BaseQuery(string.Empty, ConnectionString, ConnectType).CreateTransaction();
        }
    }
}