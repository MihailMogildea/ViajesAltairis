namespace ViajesAltairis.ProvidersApi.Repositories;

public interface IHotelSyncRepository
{
    Task<long?> GetCityIdByNameAsync(string cityName);
    Task<dynamic?> FindHotelAsync(string hotelName, string cityName);
    Task<long> InsertHotelAsync(long cityId, string name, int stars, string address, string? email, string? phone);
    Task<long?> FindHotelProviderAsync(long hotelId, long providerId);
    Task<long> InsertHotelProviderAsync(long hotelId, long providerId);
    Task<long?> GetRoomTypeIdAsync(string name);
    Task<long?> GetCurrencyIdByCodeAsync(string code);
    Task<long?> GetCurrentExchangeRateIdAsync(long currencyId);
    Task<long> UpsertHotelProviderRoomTypeAsync(long hotelProviderId, long roomTypeId, int capacity, int quantity, decimal pricePerNight, long currencyId, long exchangeRateId);
    Task<long?> GetBoardTypeIdAsync(string name);
    Task UpsertBoardPriceAsync(long hprtId, long boardTypeId, decimal pricePerNight);
    Task<IEnumerable<dynamic>> SearchHotelsAsync(string? city, int? stars);
    Task<dynamic?> GetHotelDetailAsync(long id);
    Task<IEnumerable<dynamic>> GetRoomTypesForHotelProviderAsync(long hotelId, long providerId);
    Task<IEnumerable<dynamic>> GetBoardsForRoomTypeAsync(long hprtId);
}
