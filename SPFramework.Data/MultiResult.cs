using SPFramework.Data.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SPFramework.Data
{
    public class MultiResult
    {
        private readonly ArrayList _resultList = new ArrayList();

        /// <summary>
        /// This method fetches a single result
        /// </summary>
        /// <typeparam name="T">The type of object the result is</typeparam>
        /// <param name="index">The index in the list of results this result is located</param>
        /// <returns>A single result of type T</returns>
        ///
        public T Fetch<T>(int index)
        {
            try
            {
                if (null == _resultList[index]) { return default(T); } //Guard clause
                return (T)_resultList[index];
            }
            catch (IndexOutOfRangeException e)
            {
                throw new SPSqlException("No result found at index" + index.ToString(), e);
            }
            catch (InvalidCastException ex)
            {
                throw new SPSqlException("The result is not of the type " + typeof(T), ex);
            }
        }

        /// <summary>
        /// This method fetches a resultset that is a List of results and not just a single object
        /// </summary>
        /// <typeparam name="T">The type of objects the list contains</typeparam>
        /// <param name="index">The index at which you want to get the results from the collection</param>
        /// <returns>A strongly type list of T objects</returns>
        ///
        public List<T> FetchAll<T>(int index)
        {
            List<T> resultset = new List<T>();
            try
            {
                var results = _resultList[index];
                if (null == results) { return new List<T>(); } //Guard clause

                var list = results as ArrayList;
                if (list != null)
                {
                    resultset.InsertRange(0, list.Cast<T>());
                }
                else
                {
                    resultset.Add((T)results);
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new SPSqlException("No result found at index" + index.ToString(), e);
            }
            catch (InvalidCastException ex)
            {
                throw new SPSqlException("The result is not of the type " + typeof(T), ex);
            }
            return resultset;
        }

        internal void AddResult(object result)
        {
            _resultList.Add(result);
        }

        /// <summary>
        /// Returns the number of results returned from the queries
        /// </summary>
        ///
        public int Count => _resultList.Count;
    }
}