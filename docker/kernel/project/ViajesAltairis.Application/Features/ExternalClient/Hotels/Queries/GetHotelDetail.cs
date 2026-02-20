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

            roomDtos = rooms.Select(r => new RoomCatalogDto(
                r.HotelProviderRoomTypeId, r.RoomTypeName, r.ProviderName,
                r.Capacity, r.Quantity, r.PricePerNight, r.CurrencyCode, r.Enabled,
                boardsByRoom.GetValueOrDefault(r.HotelProviderRoomTypeId, [])
                    .Select(b => new BoardOptionDto(b.HotelProviderRoomTypeBoardId, b.BoardTypeName, b.PricePerNight, b.Enabled))
                    .ToList()
            )).ToList();
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

    private record HotelDetailRow(
        long HotelId, string HotelName, int Stars, string Address,
        string? Email, string? Phone, TimeOnly CheckInTime, TimeOnly CheckOutTime,
        decimal? Latitude, decimal? Longitude,
        string CityName, string AdminDivisionName, string CountryName,
        decimal? AvgRating, int ReviewCount,
        int? FreeCancellationHours, decimal? PenaltyPercentage);

    private record RoomRow(
        long HotelProviderRoomTypeId, string RoomTypeName, string ProviderName,
        int Capacity, int Quantity, decimal PricePerNight, string CurrencyCode, bool Enabled);

    private record BoardRow(
        long HotelProviderRoomTypeBoardId, long HotelProviderRoomTypeId,
        string BoardTypeName, decimal PricePerNight, bool Enabled);
}
