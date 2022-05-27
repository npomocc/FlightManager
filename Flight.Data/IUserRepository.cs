using Flight.Domain;

namespace Flight.Data;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> Auth(string userName, string password, CancellationToken cancellationToken);
}