using System.Collections;
using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace ViajesAltairis.ExternalClient.Api.Tests.Helpers;

/// <summary>
/// Wraps a SqliteConnection so that Dapper's constructor-based record materialization
/// works correctly. SQLite returns Int64/Double/String where .NET records expect
/// bool/decimal/int/DateTime/DateOnly/TimeOnly. This wrapper intercepts GetFieldType()
/// and GetValue() to report the correct .NET types.
/// </summary>
internal sealed class TypeCoercingConnection : DbConnection
{
    private readonly SqliteConnection _inner;

    public TypeCoercingConnection(SqliteConnection inner) => _inner = inner;

    public override string ConnectionString
    {
        get => _inner.ConnectionString;
        set => _inner.ConnectionString = value;
    }

    public override string Database => _inner.Database;
    public override string DataSource => _inner.DataSource;
    public override string ServerVersion => _inner.ServerVersion;
    public override ConnectionState State => _inner.State;

    public override void Open() => _inner.Open();
    public override void Close() { } // Don't close the shared connection
    public override void ChangeDatabase(string databaseName) => _inner.ChangeDatabase(databaseName);

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        => _inner.BeginTransaction(isolationLevel);

    protected override DbCommand CreateDbCommand()
        => new TypeCoercingCommand(_inner.CreateCommand());

    protected override void Dispose(bool disposing)
    {
        // Don't dispose the inner connection — SqliteTestDatabase owns it
        base.Dispose(disposing);
    }
}

internal sealed class TypeCoercingCommand : DbCommand
{
    private readonly SqliteCommand _inner;

    public TypeCoercingCommand(SqliteCommand inner) => _inner = inner;

    public override string CommandText
    {
        get => _inner.CommandText;
        set => _inner.CommandText = value;
    }

    public override int CommandTimeout
    {
        get => _inner.CommandTimeout;
        set => _inner.CommandTimeout = value;
    }

    public override CommandType CommandType
    {
        get => _inner.CommandType;
        set => _inner.CommandType = value;
    }

    public override bool DesignTimeVisible
    {
        get => _inner.DesignTimeVisible;
        set => _inner.DesignTimeVisible = value;
    }

    public override UpdateRowSource UpdatedRowSource
    {
        get => _inner.UpdatedRowSource;
        set => _inner.UpdatedRowSource = value;
    }

    protected override DbConnection? DbConnection
    {
        get => _inner.Connection;
        set => _inner.Connection = value as SqliteConnection;
    }

    protected override DbTransaction? DbTransaction
    {
        get => _inner.Transaction;
        set => _inner.Transaction = value as SqliteTransaction;
    }

    protected override DbParameterCollection DbParameterCollection => _inner.Parameters;

    protected override DbParameter CreateDbParameter() => _inner.CreateParameter();

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        => new TypeCoercingReader(_inner.ExecuteReader(behavior));

    protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(
        CommandBehavior behavior, CancellationToken cancellationToken)
        => new TypeCoercingReader(await _inner.ExecuteReaderAsync(behavior, cancellationToken));

    public override int ExecuteNonQuery() => _inner.ExecuteNonQuery();
    public override object? ExecuteScalar() => _inner.ExecuteScalar();

    public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        => await _inner.ExecuteNonQueryAsync(cancellationToken);

    public override async Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken)
        => await _inner.ExecuteScalarAsync(cancellationToken);

    public override void Cancel() => _inner.Cancel();
    public override void Prepare() => _inner.Prepare();

    protected override void Dispose(bool disposing)
    {
        if (disposing) _inner.Dispose();
        base.Dispose(disposing);
    }
}

internal sealed class TypeCoercingReader : DbDataReader
{
    private readonly DbDataReader _inner;
    private Dictionary<int, Type>? _columnTypes;

    // Map column names (case-insensitive) to the .NET types that Dapper records expect.
    // Covers both snake_case (raw SQL columns) and PascalCase (handler aliases).
    private static readonly Dictionary<string, Type> TypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        // bool
        ["enabled"] = typeof(bool),
        ["partner_enabled"] = typeof(bool),
        ["visible"] = typeof(bool),
        // decimal
        ["subtotal"] = typeof(decimal),
        ["tax_amount"] = typeof(decimal),
        ["TaxAmount"] = typeof(decimal),
        ["margin_amount"] = typeof(decimal),
        ["discount_amount"] = typeof(decimal),
        ["DiscountAmount"] = typeof(decimal),
        ["total_price"] = typeof(decimal),
        ["TotalPrice"] = typeof(decimal),
        ["price_per_night"] = typeof(decimal),
        ["PricePerNight"] = typeof(decimal),
        ["board_price_per_night"] = typeof(decimal),
        ["BoardPricePerNight"] = typeof(decimal),
        ["avg_rating"] = typeof(decimal),
        ["AvgRating"] = typeof(decimal),
        ["penalty_percentage"] = typeof(decimal),
        ["PenaltyPercentage"] = typeof(decimal),
        ["margin"] = typeof(decimal),
        ["hotel_margin"] = typeof(decimal),
        ["latitude"] = typeof(decimal),
        ["longitude"] = typeof(decimal),
        // int
        ["stars"] = typeof(int),
        ["line_count"] = typeof(int),
        ["LineCount"] = typeof(int),
        ["num_rooms"] = typeof(int),
        ["NumRooms"] = typeof(int),
        ["num_guests"] = typeof(int),
        ["NumGuests"] = typeof(int),
        ["num_nights"] = typeof(int),
        ["NumNights"] = typeof(int),
        ["review_count"] = typeof(int),
        ["ReviewCount"] = typeof(int),
        ["free_cancellation_hours"] = typeof(int),
        ["FreeCancellationHours"] = typeof(int),
        ["capacity"] = typeof(int),
        ["quantity"] = typeof(int),
        ["TotalRooms"] = typeof(int),
        ["AvailableRooms"] = typeof(int),
        ["available_rooms"] = typeof(int),
        // DateTime
        ["created_at"] = typeof(DateTime),
        ["CreatedAt"] = typeof(DateTime),
        ["updated_at"] = typeof(DateTime),
        // DateOnly
        ["check_in_date"] = typeof(DateOnly),
        ["CheckInDate"] = typeof(DateOnly),
        ["check_out_date"] = typeof(DateOnly),
        ["CheckOutDate"] = typeof(DateOnly),
        // TimeOnly
        ["check_in_time"] = typeof(TimeOnly),
        ["CheckInTime"] = typeof(TimeOnly),
        ["check_out_time"] = typeof(TimeOnly),
        ["CheckOutTime"] = typeof(TimeOnly),
    };

    public TypeCoercingReader(DbDataReader inner) => _inner = inner;

    private Dictionary<int, Type> ColumnTypes
    {
        get
        {
            if (_columnTypes == null)
            {
                _columnTypes = new();
                for (var i = 0; i < _inner.FieldCount; i++)
                {
                    if (TypeMap.TryGetValue(_inner.GetName(i), out var t))
                        _columnTypes[i] = t;
                }
            }
            return _columnTypes;
        }
    }

    private bool TryCoerce(int ordinal, out Type type) =>
        ColumnTypes.TryGetValue(ordinal, out type!);

    public override Type GetFieldType(int ordinal) =>
        TryCoerce(ordinal, out var t) ? t : _inner.GetFieldType(ordinal);

    public override object GetValue(int ordinal)
    {
        var raw = _inner.GetValue(ordinal);
        if (raw is DBNull || !TryCoerce(ordinal, out var t)) return raw;
        return CoerceTo(raw, t);
    }

    public override int GetValues(object[] values)
    {
        var count = _inner.GetValues(values);
        for (var i = 0; i < count; i++)
        {
            if (values[i] is not DBNull && TryCoerce(i, out var t))
                values[i] = CoerceTo(values[i], t);
        }
        return count;
    }

    private static object CoerceTo(object value, Type target)
    {
        // For nullable types, coerce to the underlying type (boxing handles the rest)
        var actual = Nullable.GetUnderlyingType(target) ?? target;
        return actual switch
        {
            _ when actual == typeof(bool) => Convert.ToBoolean(value),
            _ when actual == typeof(decimal) => Convert.ToDecimal(value),
            _ when actual == typeof(int) => Convert.ToInt32(value),
            _ when actual == typeof(long) => Convert.ToInt64(value),
            _ when actual == typeof(DateTime) => DateTime.Parse((string)value),
            _ when actual == typeof(DateOnly) => DateOnly.Parse((string)value),
            _ when actual == typeof(TimeOnly) => TimeOnly.Parse((string)value),
            _ => Convert.ChangeType(value, actual),
        };
    }

    // Typed accessors — delegate or coerce as needed
    public override bool GetBoolean(int ordinal) =>
        TryCoerce(ordinal, out _) ? Convert.ToBoolean(_inner.GetValue(ordinal)) : _inner.GetBoolean(ordinal);
    public override decimal GetDecimal(int ordinal) =>
        TryCoerce(ordinal, out _) ? Convert.ToDecimal(_inner.GetValue(ordinal)) : _inner.GetDecimal(ordinal);
    public override int GetInt32(int ordinal) =>
        TryCoerce(ordinal, out _) ? Convert.ToInt32(_inner.GetValue(ordinal)) : _inner.GetInt32(ordinal);
    public override DateTime GetDateTime(int ordinal) =>
        TryCoerce(ordinal, out _) ? DateTime.Parse(_inner.GetString(ordinal)) : _inner.GetDateTime(ordinal);

    // Direct delegation
    public override int FieldCount => _inner.FieldCount;
    public override bool HasRows => _inner.HasRows;
    public override bool IsClosed => _inner.IsClosed;
    public override int Depth => _inner.Depth;
    public override int RecordsAffected => _inner.RecordsAffected;
    public override object this[int ordinal] => GetValue(ordinal);
    public override object this[string name] => GetValue(GetOrdinal(name));

    public override string GetName(int ordinal) => _inner.GetName(ordinal);
    public override int GetOrdinal(string name) => _inner.GetOrdinal(name);
    public override string GetDataTypeName(int ordinal) => _inner.GetDataTypeName(ordinal);
    public override bool IsDBNull(int ordinal) => _inner.IsDBNull(ordinal);

    public override byte GetByte(int ordinal) => _inner.GetByte(ordinal);
    public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
        => _inner.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
    public override char GetChar(int ordinal) => _inner.GetChar(ordinal);
    public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
        => _inner.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
    public override double GetDouble(int ordinal) => _inner.GetDouble(ordinal);
    public override float GetFloat(int ordinal) => _inner.GetFloat(ordinal);
    public override Guid GetGuid(int ordinal) => _inner.GetGuid(ordinal);
    public override short GetInt16(int ordinal) => _inner.GetInt16(ordinal);
    public override long GetInt64(int ordinal) => _inner.GetInt64(ordinal);
    public override string GetString(int ordinal) => _inner.GetString(ordinal);

    public override bool Read() => _inner.Read();
    public override bool NextResult() => _inner.NextResult();
    public override Task<bool> ReadAsync(CancellationToken cancellationToken) => _inner.ReadAsync(cancellationToken);
    public override Task<bool> NextResultAsync(CancellationToken cancellationToken) => _inner.NextResultAsync(cancellationToken);

    public override IEnumerator GetEnumerator() => _inner.GetEnumerator();

    protected override void Dispose(bool disposing)
    {
        if (disposing) _inner.Dispose();
        base.Dispose(disposing);
    }
}
