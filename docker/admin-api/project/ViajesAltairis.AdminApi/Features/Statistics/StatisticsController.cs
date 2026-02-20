using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.Statistics.Queries;

namespace ViajesAltairis.AdminApi.Features.Statistics;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly ISender _sender;
    public StatisticsController(ISender sender) => _sender = sender;

    [HttpGet("revenue/by-hotel")]
    public async Task<IActionResult> RevenueByHotel([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _sender.Send(new GetRevenueByHotelQuery(from, to)));

    [HttpGet("revenue/by-provider")]
    public async Task<IActionResult> RevenueByProvider([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _sender.Send(new GetRevenueByProviderQuery(from, to)));

    [HttpGet("revenue/by-period")]
    public async Task<IActionResult> RevenueByPeriod([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string groupBy = "month")
        => Ok(await _sender.Send(new GetRevenueByPeriodQuery(from, to, groupBy)));

    [HttpGet("bookings/volume")]
    public async Task<IActionResult> BookingVolume([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string groupBy = "month")
        => Ok(await _sender.Send(new GetBookingVolumeQuery(from, to, groupBy)));

    [HttpGet("bookings/by-status")]
    public async Task<IActionResult> BookingsByStatus([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _sender.Send(new GetBookingsByStatusQuery(from, to)));

    [HttpGet("bookings/average")]
    public async Task<IActionResult> BookingAverage([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _sender.Send(new GetBookingAverageQuery(from, to)));

    [HttpGet("occupancy")]
    public async Task<IActionResult> Occupancy([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _sender.Send(new GetOccupancyQuery(from, to)));

    [HttpGet("users/growth")]
    public async Task<IActionResult> UserGrowth([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string groupBy = "month")
        => Ok(await _sender.Send(new GetUserGrowthQuery(from, to, groupBy)));

    [HttpGet("users/by-type")]
    public async Task<IActionResult> UsersByType([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _sender.Send(new GetUsersByTypeQuery(from, to)));

    [HttpGet("users/subscriptions")]
    public async Task<IActionResult> ActiveSubscriptions()
        => Ok(await _sender.Send(new GetActiveSubscriptionsQuery()));

    [HttpGet("reviews")]
    public async Task<IActionResult> Reviews([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _sender.Send(new GetReviewStatsQuery(from, to)));

    [HttpGet("cancellations")]
    public async Task<IActionResult> Cancellations([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _sender.Send(new GetCancellationStatsQuery(from, to)));

    [HttpGet("promo-codes")]
    public async Task<IActionResult> PromoCodes([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _sender.Send(new GetPromoCodeStatsQuery(from, to)));

    [HttpGet("subscriptions/mrr")]
    public async Task<IActionResult> SubscriptionMrr()
        => Ok(await _sender.Send(new GetSubscriptionMrrQuery()));
}
