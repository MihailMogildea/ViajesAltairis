using System.Data;
using System.Data.Common;

namespace ViajesAltairis.Reservations.Api.Tests.Helpers;

/// <summary>
/// Utility class for mocking IDbConnection to work with Dapper's async extension methods.
/// Dapper requires DbConnection/DbCommand (abstract classes), not IDbConnection/IDbCommand interfaces.
/// </summary>
public class DapperMockHelper
{
    private readonly Queue<DataTable> _queryResults = new();
    private readonly Queue<object> _scalarResults = new();
    private readonly Queue<int> _executeResults = new();

    public DapperMockHelper EnqueueQuery(DataTable table)
    {
        _queryResults.Enqueue(table);
        return this;
    }

    public DapperMockHelper EnqueueEmptyQuery(params string[] columnNames)
    {
        var table = new DataTable();
        foreach (var col in columnNames)
            table.Columns.Add(col, typeof(object));
        _queryResults.Enqueue(table);
        return this;
    }

    public DapperMockHelper EnqueueSingleRow(params (string name, object value)[] columns)
    {
        var table = new DataTable();
        var values = new object[columns.Length];
        for (int i = 0; i < columns.Length; i++)
        {
            // DBNull.Value.GetType() returns typeof(DBNull) which DataTable rejects
            var colType = columns[i].value is null || columns[i].value is DBNull
                ? typeof(object)
                : columns[i].value.GetType();
            table.Columns.Add(columns[i].name, colType);
            values[i] = columns[i].value ?? DBNull.Value;
        }
        table.Rows.Add(values);
        return EnqueueQuery(table);
    }

    /// <summary>
    /// Enqueue a scalar result. Dapper's ExecuteScalarAsync uses ExecuteReaderAsync internally,
    /// so we enqueue a single-row DataTable.
    /// </summary>
    public DapperMockHelper EnqueueScalar<T>(T value)
    {
        var table = new DataTable();
        table.Columns.Add("value", typeof(T));
        table.Rows.Add(value!);
        _queryResults.Enqueue(table);
        return this;
    }

    public DapperMockHelper EnqueueExecute(int rowsAffected = 1)
    {
        _executeResults.Enqueue(rowsAffected);
        return this;
    }

    public DapperMockHelper EnqueueMultiRow(string[] columnNames, params object[][] rows)
    {
        var table = new DataTable();
        foreach (var col in columnNames)
            table.Columns.Add(col, typeof(object));
        foreach (var row in rows)
            table.Rows.Add(row);
        return EnqueueQuery(table);
    }

    /// <summary>
    /// Build a FakeDbConnection that Dapper's async methods can use.
    /// </summary>
    public FakeDbConnection BuildConnection()
    {
        return new FakeDbConnection(_queryResults, _scalarResults, _executeResults);
    }
}

/// <summary>
/// Concrete DbConnection subclass that Dapper's async methods accept.
/// </summary>
public class FakeDbConnection : DbConnection
{
    private readonly Queue<DataTable> _queryResults;
    private readonly Queue<object> _scalarResults;
    private readonly Queue<int> _executeResults;
    private ConnectionState _state = ConnectionState.Open;

    public FakeDbConnection(Queue<DataTable> queryResults, Queue<object> scalarResults, Queue<int> executeResults)
    {
        _queryResults = queryResults;
        _scalarResults = scalarResults;
        _executeResults = executeResults;
    }

    public override string ConnectionString { get; set; } = "";
    public override string Database => "test";
    public override string DataSource => "test";
    public override string ServerVersion => "1.0";
    public override ConnectionState State => _state;

    public override void ChangeDatabase(string databaseName) { }
    public override void Close() => _state = ConnectionState.Closed;
    public override void Open() => _state = ConnectionState.Open;

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) =>
        throw new NotImplementedException();

    protected override DbCommand CreateDbCommand() =>
        new FakeDbCommand(_queryResults, _scalarResults, _executeResults, this);
}

/// <summary>
/// Concrete DbCommand subclass that provides DataTableReader results for Dapper.
/// </summary>
public class FakeDbCommand : DbCommand
{
    private readonly Queue<DataTable> _queryResults;
    private readonly Queue<object> _scalarResults;
    private readonly Queue<int> _executeResults;
    private readonly FakeDbParameterCollection _parameters = new();

    public FakeDbCommand(Queue<DataTable> queryResults, Queue<object> scalarResults, Queue<int> executeResults, DbConnection connection)
    {
        _queryResults = queryResults;
        _scalarResults = scalarResults;
        _executeResults = executeResults;
        DbConnection = connection;
    }

    public override string CommandText { get; set; } = "";
    public override int CommandTimeout { get; set; }
    public override CommandType CommandType { get; set; }
    public override bool DesignTimeVisible { get; set; }
    public override UpdateRowSource UpdatedRowSource { get; set; }
    protected override DbConnection? DbConnection { get; set; }
    protected override DbParameterCollection DbParameterCollection => _parameters;
    protected override DbTransaction? DbTransaction { get; set; }

    public override void Cancel() { }
    public override int ExecuteNonQuery() =>
        _executeResults.Count > 0 ? _executeResults.Dequeue() : 1;

    public override object? ExecuteScalar()
    {
        if (_scalarResults.Count > 0) return _scalarResults.Dequeue();
        // Dapper's ExecuteScalarAsync may call ExecuteScalar() instead of ExecuteReaderAsync()
        // depending on the version. Fall back to reading first column of first row from query queue.
        if (_queryResults.Count > 0)
        {
            var table = _queryResults.Dequeue();
            if (table.Rows.Count > 0 && table.Columns.Count > 0)
                return table.Rows[0][0];
            return null;
        }
        return null;
    }

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) =>
        _queryResults.Count > 0
            ? _queryResults.Dequeue().CreateDataReader()
            : new DataTable().CreateDataReader();

    public override void Prepare() { }

    protected override DbParameter CreateDbParameter() => new FakeDbParameter();
}

/// <summary>
/// Minimal DbParameter implementation for Dapper parameter binding.
/// </summary>
public class FakeDbParameter : DbParameter
{
    public override DbType DbType { get; set; }
    public override ParameterDirection Direction { get; set; }
    public override bool IsNullable { get; set; }
    public override string ParameterName { get; set; } = "";
    public override int Size { get; set; }
    public override string SourceColumn { get; set; } = "";
    public override bool SourceColumnNullMapping { get; set; }
    public override object? Value { get; set; }

    public override void ResetDbType() => DbType = DbType.String;
}

/// <summary>
/// Minimal DbParameterCollection implementation.
/// </summary>
public class FakeDbParameterCollection : DbParameterCollection
{
    private readonly List<DbParameter> _parameters = [];

    public override int Count => _parameters.Count;
    public override object SyncRoot => this;

    public override int Add(object value)
    {
        _parameters.Add((DbParameter)value);
        return _parameters.Count - 1;
    }

    public override void AddRange(Array values)
    {
        foreach (DbParameter p in values) _parameters.Add(p);
    }

    public override void Clear() => _parameters.Clear();
    public override bool Contains(object value) => _parameters.Contains((DbParameter)value);
    public override bool Contains(string value) => _parameters.Any(p => p.ParameterName == value);
    public override void CopyTo(Array array, int index) { }

    public override System.Collections.IEnumerator GetEnumerator() => _parameters.GetEnumerator();

    public override int IndexOf(object value) => _parameters.IndexOf((DbParameter)value);
    public override int IndexOf(string parameterName) => _parameters.FindIndex(p => p.ParameterName == parameterName);
    public override void Insert(int index, object value) => _parameters.Insert(index, (DbParameter)value);
    public override void Remove(object value) => _parameters.Remove((DbParameter)value);
    public override void RemoveAt(int index) => _parameters.RemoveAt(index);
    public override void RemoveAt(string parameterName)
    {
        var idx = IndexOf(parameterName);
        if (idx >= 0) _parameters.RemoveAt(idx);
    }

    protected override DbParameter GetParameter(int index) => _parameters[index];
    protected override DbParameter GetParameter(string parameterName) =>
        _parameters.First(p => p.ParameterName == parameterName);
    protected override void SetParameter(int index, DbParameter value) => _parameters[index] = value;
    protected override void SetParameter(string parameterName, DbParameter value)
    {
        var idx = IndexOf(parameterName);
        if (idx >= 0) _parameters[idx] = value;
    }
}
