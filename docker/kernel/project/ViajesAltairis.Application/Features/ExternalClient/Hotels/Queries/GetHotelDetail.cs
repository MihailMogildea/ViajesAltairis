using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.ExternalClient.Hotels.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.ExternalClient.Hotels.Queries;

public record GetHotelDetailQuery(long HotelId) : IRequest<HotelDetailDto?>;

public class GetHotelDetailHandler : IRequestHandler<GetHotelDetailQuery, HotelDetailDto?>
{
    private readonly IDbConnectionFactory _db;

    public GetHotelDetailHandler(IDbConnectionFactory db) => _db = db;

    public async Task<HotelDetailDto?> Handle(GetHotelDetailQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();

        const string hotelSql = @"
            SELECT hotel_id AS HotelId, hotel_name AS HotelName, stars AS Stars,
                   address AS Address, email AS Email, phone AS Phone,
                   check_in_time AS CheckInTime, check_out_time AS CheckOutTime,
                   latitude AS Latitude, longitude AS Longitude,
                   city_name AS CityName, admin_division_name AS AdminDivisionName,
                   country_name AS CountryName, avg_rating AS AvgRating,
                   review_count AS ReviewCount, free_cancellation_hours AS FreeCancellationHours,
                   penalty_percentage AS PenaltyPercentage
            FROM v_hotel_detail
            WHERE hotel_id = @HotelId AND enabled = TRUE";

        var hotel = await connection.QuerySingleOrDefaultAsync<HotelDetailRow>(hotelSql, new { request.HotelId });
        if (hotel is null)
            return null;

        const string roomsSql = @"
            SELECT hotel_provider_room_type_id AS HotelProviderRoomTypeId,
                   room_type_name AS RoomTypeName, provider_name AS ProviderName,
                   capacity AS Capacity, quantity AS Quantity,
                   price_per_night AS PricePerNight, currency_code AS CurrencyCode,
                   provider_margin AS ProviderMargin, hotel_margin AS HotelMargin,
                   enabled AS Enabled
            FROM v_hotel_room_catalog
            WHERE hotel_id = @HotelId AND enabled = TRUE";

        var rooms = (await connection.QueryAsync<RoomRow>(roomsSql, new { request.HotelId })).ToList();

        var roomDtos = new List<RoomCatalogDto>();

        if (rooms.Count > 0)
        {
            var roomIds = rooms.Select(r => r.HotelProviderRoomTypeId).ToList();

            const string boardsSql = @"
                SELECT hotel_provider_room_type_board_id AS HotelProviderRoomTypeBoardId,
                       hotel_provider_room_type_id AS HotelProviderRoomTypeId,
                       board_type_name AS BoardTypeName,
                       price_per_night AS PricePerNight,
                       enabled AS Enabled
                FROM v_room_board_option
                WHERE hotel_provider_room_type_id IN @RoomIds AND enabled = TRUE";

            var boards = (await connection.QueryAsync<BoardRow>(boardsSql, new { RoomIds = roomIds })).ToList();
            var boardsByRoom = boards.GroupBy(b => b.HotelProviderRoomTypeId)
                                     .ToDictionary(g => g.Key, g => g.ToList());

            // Apply provider + hotel margins (no dates available for seasonal)
            roomDtos = rooms.Select(r =>
            {
                var marginFactor = 1 + (r.ProviderMargin + r.HotelMargin) / 100m;
                return new RoomCatalogDto(
                    r.HotelProviderRoomTypeId, r.RoomTypeName, r.ProviderName,
                    r.Capacity, r.Quantity,
                    Math.Round(r.PricePerNight * marginFactor, 2),
                    r.CurrencyCode, r.Enabled,
                    boardsByRoom.GetValueOrDefault(r.HotelProviderRoomTypeId, [])
                        .Select(b => new BoardOptionDto(
                            b.HotelProviderRoomTypeBoardId, b.BoardTypeName,
                            Math.Round(b.PricePerNight * marginFactor, 2), b.Enabled))
                        .ToList());
            }).ToList();
        }

        const string amenitiesSql = @"
            SELECT amenity_name AS AmenityName, amenity_category_name AS AmenityCategoryName
            FROM v_hotel_amenity_list
            WHERE hotel_id = @HotelId";

        var amenities = (await connection.QueryAsync<HotelAmenityDto>(amenitiesSql, new { request.HotelId })).ToList();

        return new HotelDetailDto(
            hotel.HotelId, hotel.HotelName, hotel.Stars, hotel.Address,
            hotel.Email, hotel.Phone, hotel.CheckInTime, hotel.CheckOutTime,
            hotel.Latitude, hotel.Longitude, hotel.CityName, hotel.AdminDivisionName,
            hotel.CountryName, hotel.AvgRating, hotel.ReviewCount,
            hotel.FreeCancellationHours, hotel.PenaltyPercentage,
            roomDtos, amenities);
    }

    private class HotelDetailRow
    {
        public long HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public int Stars { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public TimeOnly CheckInTime { get; set; }
        public TimeOnly CheckOutTime { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string AdminDivisionName { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public decimal? AvgRating { get; set; }
        public int ReviewCount { get; set; }
        public int? FreeCancellationHours { get; set; }
        public decimal? PenaltyPercentage { get; set; }
    }

    private class RoomRow
    {
        public long HotelProviderRoomTypeId { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerNight { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal ProviderMargin { get; set; }
        public decimal HotelMargin { get; set; }
        public bool Enabled { get; set; }
    }

    private class BoardRow
    {
        public long HotelProviderRoomTypeBoardId { get; set; }
        public long HotelProviderRoomTypeId { get; set; }
        public string BoardTypeName { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public bool Enabled { get; set; }
    }
}
