using Flight.Domain;
using Microsoft.EntityFrameworkCore;

namespace Flight.Data
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(FlightDbContext context) : base(context) { }

        public async Task<User?> Auth(string userName, string password, CancellationToken cancellationToken)
        {
            var user = await ((FlightDbContext)Context).Users.FirstOrDefaultAsync(
                p => p.UserName == userName && p.Password == password, cancellationToken);
            return user;
        }
    }
}
