using System.Collections.Generic;

namespace SPFramework.Data.Extensions
{
    /// <summary>
    /// The purpose of this class is to allow access to a number of the methods of the child classes of BaseQuery
    /// through the same QueryBuilder object
    /// </summary>
    ///
    public static class QueryBuilderExtensionMethods
    {
        /// <summary>
        /// Executes a stored proc query that doesn't return any data
        /// </summary>
        /// <param name="qb">Implcit type being extended</param>
        ///
        public static void Execute(this QueryBuilder<StoredProcedure> qb)
        {
            qb.Query.Execute();
        }

        /// <summary>
        /// Fetches a single object filled with the data from a single record. Also returns any output parameters
        /// </summary>
        /// <typeparam name="T">The Concrete type that will be returned from the query</typeparam>
        /// <param name="qb">Implcit type being extended</param>
        /// <returns>The instance filled with the data from the query</returns>
        ///
        public static T Fetch<T>(this QueryBuilder<StoredProcedure> qb) where T : class, new()
        {
            return qb.Query.Fetch<T>();
        }

        /// <summary>
        /// Fetches a List of objects filled with the data from a resultset. Also returns any output parameters
        /// </summary>
        /// <typeparam name="T">The Concrete type that will be in the list</typeparam>
        /// <param name="qb">Implcit type being extended</param>
        /// <returns>List of instances filled with the data from the query</returns>
        ///
        public static List<T> FetchAll<T>(this QueryBuilder<StoredProcedure> qb) where T : class, new()
        {
            return qb.Query.FetchAll<T>();
        }

        /// <summary>
        /// Returns a single value from the stored proc query
        /// </summary>
        /// <typeparam name="T">The type that will be returned from the query</typeparam>
        /// <param name="qb">Implcit type being extended</param>
        /// <returns>The type specified by the caller</returns>
        ///
        public static T ExecuteScalar<T>(this QueryBuilder<StoredProcedure> qb)
        {
            return qb.Query.ExecuteScalar<T>();
        }

        /// <summary>
        /// Returns a collection of results from a Multi-Result query
        /// </summary>
        /// <param name="qb">Implcit type being extended</param>
        /// <returns>MultiResult object with the collection of results that will be retrieved strongly typed</returns>
        ///
        public static MultiResult FetchMultiple(this QueryBuilder<StoredProcedure> qb)
        {
            return qb.Query.FetchMultiple();
        }
    }
}