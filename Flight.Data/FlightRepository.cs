using Flight.Domain;

namespace Flight.Data;

public class FlightRepository : GenericRepository<AirFlight>, IFlightRepository
{
    public FlightRepository(FlightDbContext context) : base(context) { }
}