using Flight.Domain;

namespace Flight.Data;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(FlightDbContext context) : base(context) { }
}