using Flight.Contracts;

namespace Flight.Infrastructure;

public interface IJwtAuth
{
    Task<string> Authentication(LoginView loginView, CancellationToken cancellationToken);
}