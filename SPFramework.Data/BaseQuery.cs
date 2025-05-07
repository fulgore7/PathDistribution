using SPFramework.Data.Attributes;
using SPFramework.Data.Exceptions;
using SPFramework.Data.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace SPFramework.Data
{
    public enum ConnectionType
    {
        UseDirect,
        UseConfiguration,
        UseSecurityLoader
    }

    public class BaseQuery
    {
        protected List<Func<SqlDataReader, object>> RecordMappers { get; set; }
        protected SqlCommand Command { get; set; }
        protected SqlConnection Connection { get; set; }
        protected SqlTransaction Transaction { get; set; }

        public BaseQuery(string sqlStatement, string connectionString, ConnectionType connectType)
        {
            Connection = new SqlConnection(Common.SetConnectionString(connectionString, connectType));
            Command = new SqlCommand(sqlStatement, Connection)
            {
                CommandType = GetType().Name == "StoredProcedure" ? CommandType.StoredProcedure : CommandType.Text,
                CommandTimeout = 0
            };
            RecordMappers = new List<Func<SqlDataReader, object>>();
        }

        private T MapRecordToObject<T>(SqlDataReader reader) where T : class, new()
        {
            if (RecordMappers.Count > 0)
            {
                return (T)RecordMappers[0].Invoke(reader);
            }
            else
            {
                return FillObject<T>(reader);
            }
        }

        private bool IsEnum(Type t)
        {
            Type u = Nullable.GetUnderlyingType(t);
            return (u != null) && u.IsEnum;
        }

        public T FillObject<T>(SqlDataReader reader) where T : class, new()
        {
            T instance = new T();

            if (!reader.HasRows) return instance;

            PropertyInfo[] properties = typeof(T).GetProperties();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columnName = reader.GetName(i);
                PropertyInfo match = properties.FirstOrDefault(p =>
                {
                    ColumnAttribute columnAttribute = p.GetCustomAttributes(typeof(ColumnAttribute), false)
                                                       .FirstOrDefault(a => ((ColumnAttribute)a).Name == columnName) as ColumnAttribute;
                    string propertyName = columnAttribute?.Name.ToUpper() ?? p.Name.ToUpper();
                    return columnName.ToUpper() == propertyName;
                });

                if (match != null)
                {
                    object value = reader[columnName];

                    if (match.PropertyType.IsEnum)
                    {
                        double outNum;
                        object obj = null;

                        if (double.TryParse(Convert.ToString(value), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out outNum))
                        {
                            obj = Enum.ToObject(match.PropertyType, value);
                        }
                        else
                        {
                            obj = Enum.ToObject(match.PropertyType, Convert.ChangeType(value, match.PropertyType, CultureInfo.InvariantCulture));
                        }
                        match.SetValue(instance, obj, new object[] { });
                    }
                    else if (IsEnum(match.PropertyType))
                    {
                        double outNum;
                        object obj = null;

                        if (double.TryParse(Convert.ToString(value), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out outNum))
                        {
                            obj = Enum.ToObject(Nullable.GetUnderlyingType(match.PropertyType), value);
                        }
                        else
                        {
                            obj = Enum.ToObject(Nullable.GetUnderlyingType(match.PropertyType), Convert.ChangeType(value, Nullable.GetUnderlyingType(match.PropertyType), CultureInfo.InvariantCulture));
                        }
                        match.SetValue(instance, obj, new object[] { });
                    }
                    else
                    {
                        var t = Nullable.GetUnderlyingType(match.PropertyType) ?? match.PropertyType;
                        var safeValue = (value == null || Convert.IsDBNull(value)) ? null : Convert.ChangeType(value, t, CultureInfo.InvariantCulture);
                        match.SetValue(instance, safeValue, new object[] { });
                    }
                }
            }

            return instance;
        }

        public void Map(Func<SqlDataReader, object> objectRecordMapper)
        {
            RecordMappers.Add(objectRecordMapper);
        }

        public int Map<T>() where T : class, new()
        {
            RecordMappers.Add(FillObject<T>);
            return 0;
        }

        public void WithTransaction(SqlTransaction transaction)
        {
            Transaction = transaction;
        }

        /// <summary>
        /// Creates a connection to the Database and returns a new SimpleTransaction object to use
        /// with other sql calls
        /// </summary>
        /// <returns>A new SimpleTransaction instance</returns>
        ///
        internal SPTransaction CreateTransaction()
        {
            SqlTransaction tran;
            try
            {
                Open();
                tran = Connection.BeginTransaction();
            }
            catch (Exception ex)
            {
                Close();
                throw new SPSqlException("A SimpleSql Transaction Creation Error Ocurred: ", ex);
            }
            return (new SPTransaction(tran));
        }

        /// <summary>
        /// This method is called internally that actually does the heavy lifting of adding the parameter
        ///
        /// </summary>
        /// <param name="parameterName">The name of the database parameter</param>
        /// <param name="value">The value for the parameter</param>
        /// <param name="paramType">The DB type of the parameter</param>
        ///
        public void AddParameter(string parameterName, object value, SqlDbType paramType)
        {
            AddParameter(parameterName, value, paramType, 0);
        }

        /// <summary>
        /// This method is called internally that actually does the heavy lifting of adding the parameter
        ///
        /// </summary>
        /// <param name="parameterName">The name of the database parameter</param>
        /// <param name="value">The value for the parameter</param>
        /// <param name="paramType">The DB type of the parameter</param>
        /// <param name="paramSize">The size of the data that can be stored in this parameter</param>
        ///
        public void AddParameter(string parameterName, object value, SqlDbType paramType, int paramSize)
        {
            SqlParameter param = Command.CreateParameter();
            param.ParameterName = parameterName;
            param.SqlDbType = paramType;
            param.Direction = ParameterDirection.Input;

            if (paramSize != 0
                && (param.DbType == DbType.String
                    || param.DbType == DbType.AnsiString
                    || param.DbType == DbType.StringFixedLength
                    || param.DbType == DbType.AnsiStringFixedLength))
            {
                param.Size = paramSize;
            }

            param.Value = DBNull.Value;
            if (value != null && !IsEmptyGuid(value))
            {
                param.Value = value;
            }

            Command.Parameters.Add(param);
        }

        private bool IsEmptyGuid(object obj)
        {
            if (obj == null || obj == DBNull.Value)
                return true;
            string str = obj.ToString();
            if (str == string.Empty)
                return true;
            try
            {
                Guid g = XmlConvert.ToGuid(str);
                if (g == Guid.Empty)
                    return true;
            }
            catch
            {
                // ignored
            }
            return false;
        }

        public void BulkInsert<T>(IList<T> list)
        {
            if (GetType().Name == "BulkCopy")
            {
                using (var bulkCopy = new SqlBulkCopy(Connection))
                {
                    bulkCopy.BatchSize = 10000;
                    bulkCopy.BulkCopyTimeout = 6000;
                    bulkCopy.DestinationTableName = Command.CommandText;

                    var table = new DataTable();

                    var props = TypeDescriptor.GetProperties(typeof(T))
                                               .Cast<PropertyDescriptor>()
                                               .Where(propertyInfo => propertyInfo.PropertyType.Namespace == "System")
                                               .ToArray();

                    foreach (var propertyInfo in props)
                    {
                        bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                        table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                    }

                    var values = new object[props.Length];

                    foreach (var item in list)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }
                        table.Rows.Add(values);
                    }

                    try
                    {
                        Open();
                        bulkCopy.WriteToServer(table);
                    }
                    catch (Exception ex)
                    {
                        throw new SPSqlException("A SimpleSql Save Exception Ocurred: ", ex);
                    }
                    finally
                    {
                        Close();
                    }
                }
            }
        }

        /// <summary>
        /// This method is in charge of Inserts and Updates that don't need to return any data
        /// </summary>
        ///
        public void Execute()
        {
            try
            {
                Open();

                Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new SPSqlException("A SimpleSql Save Exception Ocurred: ", ex);
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// This method returns a single scalar value cast to the proper type via generics
        /// </summary>
        /// <returns>A single scalar value</returns>
        ///
        public T ExecuteScalar<T>()
        {
            T scalarVal;
            try
            {
                Open();

                scalarVal = (T)Command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw new SPSqlException("A SimpleSql ExecuteScalar Exception Ocurred: ", ex);
            }
            finally
            {
                Close();
            }

            return scalarVal;
        }

        /// <summary>
        /// Grab a single record from the database and fill an object
        /// </summary>
        /// <typeparam name="T">The type of object to fill</typeparam>
        /// <returns>The object filled with data from the sql query or stored proc</returns>
        ///
        public T Fetch<T>() where T : class, new()
        {
            T obj = default(T);

            try
            {
                Open();

                SqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

                while (reader.Read())
                {
                    obj = MapRecordToObject<T>(reader);
                    break;
                }
            }
            catch (Exception ex)
            {
                throw new SPSqlException("A SimpleSql Fetch Exception Ocurred: ", ex);
            }
            finally
            {
                Close();
            }

            return obj;
        }

        /// <summary>
        /// Grabs a complete resultset and maps it to the object specified in the Map() method
        /// </summary>
        /// <typeparam name="T">The type of object you want to fill and return</typeparam>
        /// <returns>A list of hydrated obejcts</returns>
        ///
        public List<T> FetchAll<T>() where T : class, new()
        {
            List<T> objectList = new List<T>();
            try
            {
                Open();

                SqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

                while (reader.Read())
                {
                    objectList.Add(MapRecordToObject<T>(reader));
                }
            }
            catch (Exception ex)
            {
                throw new SPSqlException("A SimpleSql FetchAll Exception Ocurred: ", ex);
            }
            finally
            {
                Close();
            }

            return objectList;
        }

        /// <summary>
        /// This method allows you to run multiple queries and return a colleciton of results.
        /// This must be used in conjunction with the Map Method to properly map each objec or list to the
        /// proper resultset
        /// </summary>
        /// <returns>A collection of resultsets</returns>
        ///
        public MultiResult FetchMultiple()
        {
            Open();

            SqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            MultiResult resultSet = new MultiResult();

            try
            {
                int index = 0;
                do
                {
                    if (index >= RecordMappers.Count) { break; } //Guard clause to keep from invoking non-existant delegate

                    ArrayList recordSet = new ArrayList();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            recordSet.Add(RecordMappers[index].Invoke(reader));
                        }
                    }
                    else
                    {
                        //recordSet.Add(RecordMappers[index].Invoke(reader));
                        recordSet.Add(null);
                    }
                    if (recordSet.Count > 0)
                        resultSet.AddResult((recordSet.Count > 1 ? recordSet : recordSet[0]));
                    else
                        resultSet.AddResult(null);

                    index++;
                } while (reader.NextResult());
            }
            catch (Exception ex)
            {
                throw new SPSqlException("A SimpleSql FetchMultiple Exception Ocurred: ", ex);
            }
            finally
            {
                Close();
            }

            return resultSet;
        }

        private void Open()
        {
            if (Connection != null &&
                Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }

        private void Close()
        {
            if (Connection != null &&
                Connection.State == ConnectionState.Open && Command.Transaction == null)
            {
                Connection.Close();
            }

            Command.Dispose();
        }
    }
}