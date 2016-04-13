using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace AdoManager
{
    public static class DataAccessObject
    {
        public static ConnectionManager Conn = ConnectionManager.GetDefaultConnection();
        private static Dictionary<string, object> _cachingQueryResults = new Dictionary<string, object>();

        #region Sync Methods

        public static IEnumerable<dynamic> GetFrom(string table, bool caching = false, params Condition[] conditions)
        {
            return GetFrom(table, new string[] { }, caching, conditions);
        }

        public static IEnumerable<dynamic> GetFrom(string table, string[] columns, bool caching = false, params Condition[] conditions)
        {
            string query = GetFormatedQuery(table, columns, conditions);

            return GetFromQuery(query, caching);
        }

        public static IEnumerable<dynamic> GetFromQuery(string query, ExpandoObject param)
        {
            var result = Conn.SqlConn.Query<dynamic>(query, param);

            return result;
        }

        public static IEnumerable<dynamic> GetFromQuery(string query, bool caching = false)
        {
            if (caching)
            {
                if (!_cachingQueryResults.ContainsKey(query))
                {
                    _cachingQueryResults[query] = Conn.SqlConn.Query<dynamic>(query);
                }
                return (IEnumerable<dynamic>)_cachingQueryResults[query];
            }

            return Conn.SqlConn.Query<dynamic>(query);
        }

        public static T ExecuteScalar<T>(string query, ExpandoObject param)
        {
            var result = Conn.SqlConn.ExecuteScalar<T>(query, param);

            return result;
        }

        public static T ExecuteScalar<T>(string query, bool caching = false)
        {
            if (caching)
            {
                if (!_cachingQueryResults.ContainsKey(query))
                {
                    _cachingQueryResults[query] = Conn.SqlConn.ExecuteScalar<T>(query);
                }
                return (T)_cachingQueryResults[query];
            }

            return Conn.SqlConn.ExecuteScalar<T>(query);
        }

        #endregion

        #region Async Methods

        public static async Task<IEnumerable<dynamic>> GetFromAsync(string table, bool caching = false, params Condition[] conditions)
        {
            return await GetFromAsync(table, new string[] { }, caching, conditions);
        }

        public static async Task<IEnumerable<dynamic>> GetFromAsync(string table, string[] columns, bool caching = false, params Condition[] conditions)
        {
            string query = GetFormatedQuery(table, columns, conditions);

            return await GetFromQueryAsync(query, caching);
        }

        public static async Task<IEnumerable<dynamic>> GetFromQueryAsync(string query, ExpandoObject param)
        {
            var result = await Conn.SqlConn.QueryAsync<dynamic>(query, param);

            return result;
        }

        public static async Task<IEnumerable<dynamic>> GetFromQueryAsync(string query, bool caching = false)
        {
            if (caching)
            {
                if (!_cachingQueryResults.ContainsKey(query))
                {
                    _cachingQueryResults[query] = await Conn.SqlConn.QueryAsync<dynamic>(query);
                }
                return (IEnumerable<dynamic>)_cachingQueryResults[query];
            }

            return await Conn.SqlConn.QueryAsync<dynamic>(query);
        }

        public static async Task<T> ExecuteScalarAsync<T>(string query, ExpandoObject param)
        {
            var result = await Conn.SqlConn.ExecuteScalarAsync<T>(query, param);

            return result;
        }

        public static async Task<T> ExecuteScalarAsync<T>(string query, bool caching = false)
        {
            if (caching)
            {
                if (!_cachingQueryResults.ContainsKey(query))
                {
                    _cachingQueryResults[query] = await Conn.SqlConn.ExecuteScalarAsync<T>(query);
                }
                return (T)_cachingQueryResults[query];
            }

            return await Conn.SqlConn.ExecuteScalarAsync<T>(query);
        }

        #endregion

        private static string GetFormatedQuery(string table, string[] columns, Condition[] conditions)
        {
            var cols = columns.Any() ? string.Join(", ", columns) : "*";

            string query = $"SELECT {cols} FROM dbo.{table} ";

            if (conditions.Any())
            {
                query += $@"WHERE {conditions.ConvertToQuery()}";
            }

            return query;
        }

        public static void ClearCache()
        {
            _cachingQueryResults.Clear();
        }
    }
}