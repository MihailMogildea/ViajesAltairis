namespace ViajesAltairis.Domain.Entities;

public class Hotel : BaseEntity
{
    public long CityId { get; set; }
    public string Name { get; set; } = null!;
    public byte Stars { get; set; }
    public string Address { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public TimeOnly CheckInTime { get; set; }
    public TimeOnly CheckOutTime { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal Margin { get; set; }
    public bool Enabled { get; set; }

    public City City { get; set; } = null!;
    public ICollection<HotelProvider> HotelProviders { get; set; } = [];
    public ICollection<HotelAmenity> HotelAmenities { get; set; } = [];
    public ICollection<HotelImage> HotelImages { get; set; } = [];
    public ICollection<CancellationPolicy> CancellationPolicies { get; set; } = [];
    public ICollection<HotelBlackout> HotelBlackouts { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<UserHotel> UserHotels { get; set; } = [];
}
