using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace ViajesAltairis.Client.Api.Tests.Fixtures;

public static class TestDbHelper
{
    static TestDbHelper()
    {
        SqlMapper.RemoveTypeMap(typeof(decimal));
        SqlMapper.RemoveTypeMap(typeof(decimal?));
        SqlMapper.AddTypeHandler(new DecimalTypeHandler());
        SqlMapper.AddTypeHandler(new NullableDecimalTypeHandler());
        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        SqlMapper.AddTypeHandler(new NullableDateOnlyTypeHandler());
        SqlMapper.RemoveTypeMap(typeof(int));
        SqlMapper.AddTypeHandler(new IntTypeHandler());
    }

    public static SqliteConnection CreateConnection()
    {
        var conn = new SqliteConnection("Data Source=:memory:");
        conn.Open();
        conn.CreateFunction("NOW", () => DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
        conn.CreateFunction("CURDATE", () => DateTime.UtcNow.Date.ToString("yyyy-MM-dd"));
        conn.CreateFunction("CONCAT", (string? a, string? b) => (a ?? "") + (b ?? ""));
        conn.CreateFunction("CONCAT", (string? a, string? b, string? c) => (a ?? "") + (b ?? "") + (c ?? ""));
        conn.CreateFunction("CONCAT", (string? a, string? b, string? c, string? d) => (a ?? "") + (b ?? "") + (c ?? "") + (d ?? ""));
        conn.CreateFunction("LEFT", (string s, long n) => s.Length <= (int)n ? s : s[..(int)n]);
        // MariaDB LAST_INSERT_ID() → SQLite last_insert_rowid()
        conn.CreateFunction("LAST_INSERT_ID", () =>
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT last_insert_rowid()";
            return (long)cmd.ExecuteScalar()!;
        });
        return conn;
    }

    public static void CreateTable(SqliteConnection conn, string tableName, string columnsDdl)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"CREATE TABLE [{tableName}] ({columnsDdl})";
        cmd.ExecuteNonQuery();
    }

    public static void Execute(SqliteConnection conn, string sql)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    private class DecimalTypeHandler : SqlMapper.TypeHandler<decimal>
    {
        public override decimal Parse(object value) => Convert.ToDecimal(value);
        public override void SetValue(IDbDataParameter parameter, decimal value) { parameter.Value = value; }
    }

    private class NullableDecimalTypeHandler : SqlMapper.TypeHandler<decimal?>
    {
        public override decimal? Parse(object value) => value == null || value is DBNull ? null : Convert.ToDecimal(value);
        public override void SetValue(IDbDataParameter parameter, decimal? value) { parameter.Value = (object?)value ?? DBNull.Value; }
    }

    private class IntTypeHandler : SqlMapper.TypeHandler<int>
    {
        public override int Parse(object value) => Convert.ToInt32(value);
        public override void SetValue(IDbDataParameter parameter, int value) { parameter.Value = value; }
    }

    private class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override DateOnly Parse(object value) => DateOnly.Parse(value.ToString()!);
        public override void SetValue(IDbDataParameter parameter, DateOnly value) { parameter.Value = value.ToString("yyyy-MM-dd"); }
    }

    private class NullableDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly?>
    {
        public override DateOnly? Parse(object value) => value == null || value is DBNull ? null : DateOnly.Parse(value.ToString()!);
        public override void SetValue(IDbDataParameter parameter, DateOnly? value) { parameter.Value = value?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value; }
    }
}
