using Flight.Common;
using Flight.Data;
using Flight.Domain;
using System.Collections.Generic;
using System.Linq;

namespace FlightManager;

public class DataSeeder
{
    public static void SeedIdentity(FlightDbContext context)
    {
        SeedRoles(context);
        SeedUsers(context);
    }
    private static void SeedUsers(FlightDbContext context)
    {
        if (context.Users.Any()) return;
        var users = new List<User>
        {
            new() { Id = 1, RoleId = 1, UserName = "user", Password = "user".Hash("123") },
            new() { Id = 2, RoleId = 2, UserName = "mod", Password = "mod".Hash("321") }
        };
        context.AddRange(users);
        context.SaveChanges();
    }
    private static void SeedRoles(FlightDbContext context)
    {
        if (context.Roles.Any()) return;
        var roles = new List<Role>
        {
            new() { Id = 1, Code = "User" },
            new() { Id = 2, Code = "Moderator" }
        };
        context.AddRange(roles);
        context.SaveChanges();
    }
}