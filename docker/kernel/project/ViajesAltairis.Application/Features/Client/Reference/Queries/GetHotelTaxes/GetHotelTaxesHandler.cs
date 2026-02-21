using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Reference.Queries.GetHotelTaxes;

public class GetHotelTaxesHandler : IRequestHandler<GetHotelTaxesQuery, List<HotelTaxDto>>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetHotelTaxesHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<HotelTaxDto>> Handle(GetHotelTaxesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        // Resolve hotel geography: hotel → city → admin_division → country
        // Then find all enabled taxes matching any level, picking the most specific per tax type
        const string sql = """
            SELECT t.tax_type_id AS TaxTypeId, tt.name AS TaxTypeName, t.rate AS Rate, t.is_percentage AS IsPercentage,
                   CASE
                       WHEN t.city_id IS NOT NULL THEN 1
                       WHEN t.administrative_division_id IS NOT NULL THEN 2
                       ELSE 3
                   END AS Specificity
            FROM hotel h
            JOIN city c ON c.id = h.city_id
            JOIN administrative_division ad ON ad.id = c.administrative_division_id
            JOIN country co ON co.id = ad.country_id
            JOIN tax t ON t.enabled = 1
                AND (t.city_id = c.id
                  OR t.administrative_division_id = ad.id
                  OR t.country_id = co.id)
            JOIN tax_type tt ON tt.id = t.tax_type_id
            WHERE h.id = @HotelId
            ORDER BY t.tax_type_id, Specificity
            """;

        var rows = (await connection.QueryAsync<HotelTaxRow>(sql, new { request.HotelId })).ToList();

        // Most specific wins per tax type
        var seen = new HashSet<long>();
        var result = new List<HotelTaxDto>();
        foreach (var row in rows)
        {
            if (!seen.Add(row.TaxTypeId)) continue;
            result.Add(new HotelTaxDto
            {
                TaxTypeName = row.TaxTypeName,
                Rate = row.Rate,
                IsPercentage = row.IsPercentage,
            });
        }

        return result;
    }

    private class HotelTaxRow
    {
        public long TaxTypeId { get; set; }
        public string TaxTypeName { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public bool IsPercentage { get; set; }
        public int Specificity { get; set; }
    }
}
