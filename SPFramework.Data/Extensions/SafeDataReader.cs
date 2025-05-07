using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SPFramework.Data.Extensions
{
    public static class SafeDataReader
    {
        public static async Task<string> GetSafeStringAsync(this SqlDataReader reader, int colIndex)
        {
            if (!await reader.IsDBNullAsync(colIndex))
                return await reader.GetFieldValueAsync<string>(colIndex);
            else
                return default(string);
        }

        public static string GetSafeString(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetFieldValue<string>(colIndex);
            else
                return default(string);
        }

        public static async Task<int> GetSafeInt32Async(this SqlDataReader reader, int colIndex)
        {
            if (!await reader.IsDBNullAsync(colIndex))
                return await reader.GetFieldValueAsync<int>(colIndex);
            else
                return default(int);
        }

        public static int GetSafeInt32(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetFieldValue<int>(colIndex);
            else
                return default(int);
        }

        public static async Task<DateTime> GetSafeDateTimeAsync(this SqlDataReader reader, int colIndex)
        {
            if (!await reader.IsDBNullAsync(colIndex))
                return await reader.GetFieldValueAsync<DateTime>(colIndex);
            else
                return default(DateTime);
        }

        public static DateTime GetSafeDateTime(this SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetFieldValue<DateTime>(colIndex);
            else
                return default(DateTime);
        }
    }
}
