using Flight.Contracts;

namespace Flight.Infrastructure;

public interface IAuthService
{
    Task<IEnumerable<UserView>> List();
    Task<bool> AddUser(CreateUserView createView, CancellationToken cancellationToken);

    Task<bool> EditUser(UserView userView, CancellationToken cancellationToken);
    Task<UserView?> Auth(LoginView loginView, CancellationToken cancellationToken);
}