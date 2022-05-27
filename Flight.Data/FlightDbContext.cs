using Flight.Domain;
using Microsoft.EntityFrameworkCore;

namespace Flight.Data;

public class FlightDbContext : DbContext
{
    public DbSet<AirFlight> AirFlights { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    public bool UseOptions { get; set; } = false;
    public FlightDbContext(DbContextOptions<FlightDbContext> options) : base(options) { UseOptions = true; }
    //public FlightDbContext() { }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!UseOptions) optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=flights;Username=postgres;Password=gtxtymrf123");
    }
}