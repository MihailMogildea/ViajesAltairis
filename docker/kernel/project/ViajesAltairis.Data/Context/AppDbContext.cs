using Microsoft.EntityFrameworkCore;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Data.Context;

public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Currency> Currencies => Set<Currency>();
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<AdministrativeDivisionType> AdministrativeDivisionTypes => Set<AdministrativeDivisionType>();
    public DbSet<AdministrativeDivision> AdministrativeDivisions => Set<AdministrativeDivision>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<Translation> Translations => Set<Translation>();
    public DbSet<WebTranslation> WebTranslations => Set<WebTranslation>();
    public DbSet<ProviderType> ProviderTypes => Set<ProviderType>();
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<HotelProvider> HotelProviders => Set<HotelProvider>();
    public DbSet<HotelProviderRoomType> HotelProviderRoomTypes => Set<HotelProviderRoomType>();
    public DbSet<AmenityCategory> AmenityCategories => Set<AmenityCategory>();
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<HotelAmenity> HotelAmenities => Set<HotelAmenity>();
    public DbSet<HotelProviderRoomTypeAmenity> HotelProviderRoomTypeAmenities => Set<HotelProviderRoomTypeAmenity>();
    public DbSet<TaxType> TaxTypes => Set<TaxType>();
    public DbSet<Tax> Taxes => Set<Tax>();
    public DbSet<HotelImage> HotelImages => Set<HotelImage>();
    public DbSet<RoomImage> RoomImages => Set<RoomImage>();
    public DbSet<UserType> UserTypes => Set<UserType>();
    public DbSet<BusinessPartner> BusinessPartners => Set<BusinessPartner>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserHotel> UserHotels => Set<UserHotel>();
    public DbSet<ReservationStatus> ReservationStatuses => Set<ReservationStatus>();
    public DbSet<PromoCode> PromoCodes => Set<PromoCode>();
    public DbSet<BoardType> BoardTypes => Set<BoardType>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<ReservationLine> ReservationLines => Set<ReservationLine>();
    public DbSet<ReservationGuest> ReservationGuests => Set<ReservationGuest>();
    public DbSet<InvoiceStatus> InvoiceStatuses => Set<InvoiceStatus>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<PaymentTransactionFee> PaymentTransactionFees => Set<PaymentTransactionFee>();
    public DbSet<CancellationPolicy> CancellationPolicies => Set<CancellationPolicy>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<SeasonalMargin> SeasonalMargins => Set<SeasonalMargin>();
    public DbSet<SubscriptionType> SubscriptionTypes => Set<SubscriptionType>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
    public DbSet<HotelProviderRoomTypeBoard> HotelProviderRoomTypeBoards => Set<HotelProviderRoomTypeBoard>();
    public DbSet<HotelBlackout> HotelBlackouts => Set<HotelBlackout>();
    public DbSet<Cancellation> Cancellations => Set<Cancellation>();
    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ReviewResponse> ReviewResponses => Set<ReviewResponse>();
    public DbSet<JobSchedule> JobSchedules => Set<JobSchedule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
