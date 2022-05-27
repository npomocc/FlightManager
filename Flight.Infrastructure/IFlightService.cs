using Flight.Contracts;
using Flight.Domain;

namespace Flight.Infrastructure;

public interface IFlightService
{
    Task<IEnumerable<AirFlightView>> List(CancellationToken cancellationToken);
    Task<IEnumerable<AirFlightView>> Filter(AirFlightFilterView filterView, CancellationToken cancellationToken);
    Task<AirFlightView> RegisterFlight(AirFlight createView, CancellationToken cancellationToken);
    Task<bool> EditFlight(AirFlightView flightView, CancellationToken cancellationToken);
}