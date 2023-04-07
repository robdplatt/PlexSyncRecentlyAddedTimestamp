using Microsoft.Data.Sqlite;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SQLitePCL.raw;

namespace PlexSyncRecentlyAddedTimestamp.Repositories;

internal static class SqliteRepository
{
    internal static T? ReadSQLiteDataValue<T>(string dbPath, string sql)
    {
        SQLitePCL.Batteries.Init();

        using (SqliteConnection conn = new($"Data Source={dbPath};Cache=Shared"))
        {
            conn.Open();
            using SqliteCommand cmd = new(sql, conn);
            var val = cmd.ExecuteScalar();
            if (val != null)
                return (T)val;
        }

        return default;
    }

    internal static DataTable ReadSQLiteData(string dbPath, string sql)
    {
        SQLitePCL.Batteries.Init();

        DataTable dt = new();
        using (SqliteConnection conn = new($"Data Source={dbPath};Cache=Shared"))
        {
            conn.Open();
            using SqliteCommand cmd = new(sql, conn);
            using SqliteDataReader reader = cmd.ExecuteReader();
            dt.Load(reader);
        }
        return dt;
    }

    internal static void UpdateSQLiteData(string dbPath, string sql)
    {
        SQLitePCL.Batteries.Init();
        using SqliteConnection conn = new($"Data Source={dbPath};Cache=Shared");
        conn.Open();
        using SqliteCommand cmd = new(sql, conn);
        cmd.ExecuteNonQuery();
    }
}