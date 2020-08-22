﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Adapter.DB.Contract;
using SQLite;
using System.IO;

namespace Utilities.Adapter.DB
{
    public static class SQLiteConnectionMode
    {
        public const SQLiteOpenFlags ReadOnly = SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadOnly;
        public const SQLiteOpenFlags ReadWrite = SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite;
    }

    public sealed class SQLiteDBAdapter : IDBAdapter
    {
        private SQLiteConnection DBConnection;

        public bool IsDBInitialized { get { return DBConnection != null; } }

        public void Initialize(string dbfileName, string path, SQLiteOpenFlags openFlags = SQLiteConnectionMode.ReadWrite)
        {
            var dbPath = Path.Combine(path, dbfileName);
            if (!IsDBInitialized || DBConnection.DatabasePath != dbPath)
            {
                if (IsDBInitialized) { Close(); }
                DBConnection = new SQLiteConnection(dbPath, openFlags);
                DBConnection.ExecuteScalar<string>("PRAGMA journal_mode=DELETE");
            }
        }

        public void CreateTable<T>() where T : new()
        {
            ThrowIfDBNotInitialized();
            DBConnection.CreateTable<T>();
        }

        public void CreateTables(CreateFlags createFlags = CreateFlags.None, params Type[] types)
        {
            ThrowIfDBNotInitialized();
            DBConnection.CreateTables(createFlags, types);
        }

        public void DropTable<T>() where T : new()
        {
            ThrowIfDBNotInitialized();
            DBConnection.DropTable<T>();
        }

        public void DropTables(params Type[] types)
        {
            ThrowIfDBNotInitialized();
            foreach (var type in types)
            {
                var tableMapping = DBConnection.GetMapping(type);
                DBConnection.DropTable(tableMapping);
            }
        }

        public int InsertOrReplace<T>(T element) where T : new()
        {
            ThrowIfDBNotInitialized();
            return DBConnection.InsertOrReplace(element, typeof(T));
        }

        public int InsertOrReplace(object element, Type objType)
        {
            ThrowIfDBNotInitialized();
            return DBConnection.InsertOrReplace(element, objType);
        }

        public int InsertOrReplaceAll<T>(IEnumerable<T> elements) where T : new()
        {
            int count = 0;
            RunInTransaction(() =>
            {
                foreach (var element in elements)
                {
                    count += DBConnection.InsertOrReplace(element, typeof(T));
                }
            });
            return count;
        }

        public int InsertOrReplaceAll(IEnumerable<object> elements, Type objType)
        {
            int count = 0;
            RunInTransaction(() =>
            {
                foreach (var element in elements)
                {
                    count += DBConnection.InsertOrReplace(element, objType);
                }
            });
            return count;
        }

        public IList<T> Table<T>() where T : new()
        {
            ThrowIfDBNotInitialized();
            return DBConnection.Table<T>().ToList();
        }

        public IList<T> Query<T>(string query, params object[] queryParams) where T : new()
        {
            ThrowIfDBNotInitialized();
            return DBConnection.Query<T>(query, queryParams);
        }

        public int Execute(string query, params object[] args)
        {
            ThrowIfDBNotInitialized();
            return DBConnection.Execute(query, args);
        }

        public T ExecuteScalar<T>(string query, params object[] args)
        {
            ThrowIfDBNotInitialized();
            return DBConnection.ExecuteScalar<T>(query, args);
        }

        public T Find<T>(object pk) where T : new()
        {
            ThrowIfDBNotInitialized();
            return DBConnection.Find<T>(pk);
        }

        public T FindWithQuery<T>(string query, params object[] args) where T : new()
        {
            ThrowIfDBNotInitialized();
            return DBConnection.FindWithQuery<T>(query, args);
        }

        public int DeleteAll<T>() where T : new()
        {
            ThrowIfDBNotInitialized();
            return DBConnection.DeleteAll<T>();
        }

        public void RunInTransaction(Action action, bool reThrow = false)
        {
            ThrowIfDBNotInitialized();
            try
            {
                DBConnection.RunInTransaction(action);
            }
            catch (Exception)
            {
                if (reThrow) { throw; }
            }
        }

        public void RunInTransaction(Func<Task> func, bool reThrow = false)
        {
            ThrowIfDBNotInitialized();
            try
            {
                var savePoint = DBConnection.SaveTransactionPoint();
                func();
                DBConnection.Release(savePoint);
            }
            catch (Exception)
            {
                DBConnection.Rollback();
                if (reThrow) { throw; }
            }
        }

        public void Close()
        {
            ThrowIfDBNotInitialized();
            DBConnection.Close();
            DBConnection = null;
        }

        public string GetQueryParamPlaceholders(int count)
        {
            StringBuilder queryParamBuilder = new StringBuilder();
            for (var i = 0; i < count - 1; i++)
            {
                queryParamBuilder.Append("?, ");
            }
            if (count > 0) { queryParamBuilder.Append("?"); }

            return queryParamBuilder.ToString();
        }

        private void ThrowIfDBNotInitialized()
        {
            if (!IsDBInitialized)
            {
                throw new InvalidOperationException("DB not initialized");
            }
        }
    }
}
