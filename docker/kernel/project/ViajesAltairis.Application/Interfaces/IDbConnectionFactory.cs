using System.Data;

namespace ViajesAltairis.Application.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
