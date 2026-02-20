using System.Data;
using Dapper;

namespace ViajesAltairis.Admin.Api.Tests.Infrastructure;

/// <summary>
/// SQLite stores timestamps as text. This handler converts them to DateTime for Dapper.
/// </summary>
public class SqliteDateTimeHandler : SqlMapper.TypeHandler<DateTime>
{
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        parameter.Value = value.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public override DateTime Parse(object value)
    {
        return DateTime.Parse(value?.ToString() ?? "");
    }
}

/// <summary>
/// SQLite has no TimeOnly type. EF stores it as text (HH:mm:ss).
/// </summary>
public class SqliteTimeOnlyHandler : SqlMapper.TypeHandler<TimeOnly>
{
    public override void SetValue(IDbDataParameter parameter, TimeOnly value)
    {
        parameter.Value = value.ToString("HH:mm:ss");
    }

    public override TimeOnly Parse(object value)
    {
        return TimeOnly.Parse(value?.ToString() ?? "00:00:00");
    }
}

/// <summary>
/// SQLite stores booleans as integers (0/1). Dapper sometimes gets confused with record constructors.
/// </summary>
public class SqliteBoolHandler : SqlMapper.TypeHandler<bool>
{
    public override void SetValue(IDbDataParameter parameter, bool value)
    {
        parameter.Value = value ? 1L : 0L;
    }

    public override bool Parse(object value)
    {
        return Convert.ToInt64(value) != 0;
    }
}

/// <summary>
/// SQLite stores DateOnly as text.
/// </summary>
public class SqliteDateOnlyHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.Value = value.ToString("yyyy-MM-dd");
    }

    public override DateOnly Parse(object value)
    {
        return DateOnly.Parse(value?.ToString() ?? "");
    }
}
