using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Data.Repositories;

public class DapperConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DapperConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection connection string not configured.");
    }

    public IDbConnection CreateConnection() => new MySqlConnection(_connectionString);
}
