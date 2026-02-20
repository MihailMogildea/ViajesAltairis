using System.Data;
using Dapper;

namespace ViajesAltairis.Reservations.Api.Tests.Helpers;

/// <summary>
/// Dapper type handler for DateOnly, which is not natively supported.
/// Converts DateOnly to DateTime for parameter binding and back for reading.
/// </summary>
public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value.ToDateTime(TimeOnly.MinValue);
    }

    public override DateOnly Parse(object value)
    {
        return value switch
        {
            DateTime dt => DateOnly.FromDateTime(dt),
            string s => DateOnly.Parse(s),
            _ => throw new InvalidCastException($"Cannot convert {value.GetType()} to DateOnly"),
        };
    }
}
