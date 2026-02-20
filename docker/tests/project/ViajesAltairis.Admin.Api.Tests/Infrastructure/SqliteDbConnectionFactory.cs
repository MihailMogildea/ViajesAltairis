using System.Data;
using Microsoft.Data.Sqlite;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Admin.Api.Tests.Infrastructure;

public class SqliteDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqliteDbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
