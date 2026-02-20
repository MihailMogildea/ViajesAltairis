using Dapper;
using MySqlConnector;

namespace ViajesAltairis.ProvidersApi.Repositories;

public class ProviderRepository : IProviderRepository
{
    private readonly string _connectionString;

    public ProviderRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private MySqlConnection CreateConnection() => new(_connectionString);

    public async Task<IEnumerable<dynamic>> GetAllEnabledAsync()
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync("""
            SELECT p.id, p.name, pt.name AS type, p.margin, p.enabled, p.sync_status, p.last_synced_at
            FROM provider p
            JOIN provider_type pt ON pt.id = p.type_id
            WHERE p.enabled = 1
            ORDER BY p.id
            """);
    }

    public async Task<dynamic?> GetByIdAsync(long id)
    {
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync("""
            SELECT p.id, p.name, pt.name AS type, p.margin, p.enabled, p.sync_status, p.last_synced_at,
                   p.api_url, p.api_username, c.iso_code AS currency_iso_code
            FROM provider p
            JOIN provider_type pt ON pt.id = p.type_id
            JOIN currency c ON c.id = p.currency_id
            WHERE p.id = @Id
            """, new { Id = id });
    }

    public async Task<dynamic?> GetByNameAsync(string name)
    {
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync(
            "SELECT id, name, type_id, margin, enabled, sync_status FROM provider WHERE name = @Name",
            new { Name = name });
    }

    public async Task<long> InsertAsync(string name, long typeId, decimal margin)
    {
        using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<long>("""
            INSERT INTO provider (name, type_id, margin) VALUES (@Name, @TypeId, @Margin);
            SELECT LAST_INSERT_ID();
            """, new { Name = name, TypeId = typeId, Margin = margin });
    }

    public async Task<bool> TrySetSyncStatusAsync(long id, string newStatus, string? expectedCurrentStatus)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var tx = await conn.BeginTransactionAsync();

        var currentStatus = await conn.QueryFirstOrDefaultAsync<string?>(
            "SELECT sync_status FROM provider WHERE id = @Id FOR UPDATE",
            new { Id = id }, tx);

        bool canUpdate;
        if (expectedCurrentStatus == null)
            canUpdate = currentStatus == null || currentStatus != "updating";
        else
            canUpdate = currentStatus == expectedCurrentStatus;

        if (!canUpdate)
        {
            await tx.RollbackAsync();
            return false;
        }

        await conn.ExecuteAsync(
            "UPDATE provider SET sync_status = @Status WHERE id = @Id",
            new { Id = id, Status = newStatus }, tx);

        await tx.CommitAsync();
        return true;
    }

    public async Task SetSyncCompletedAsync(long id)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE provider SET sync_status = 'updated', last_synced_at = NOW() WHERE id = @Id",
            new { Id = id });
    }

    public async Task SetSyncFailedAsync(long id)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync(
            "UPDATE provider SET sync_status = 'failed' WHERE id = @Id",
            new { Id = id });
    }
}
