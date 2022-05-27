using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Flight.Common;
using Flight.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Flight.Infrastructure;

public class JwtAuth : IJwtAuth
{
    private readonly IServiceProvider _provider;
    private readonly string _key;
    public JwtAuth(string key, IServiceProvider provider)
    {
        _key = key;
        _provider = provider;
    }
    public async Task<string> Authentication(LoginView loginView, CancellationToken cancellationToken)
    {
        using var scope = _provider.CreateScope();
        var authService = scope
            .ServiceProvider
            .GetRequiredService<IAuthService>();
        var userView = await authService.Auth(loginView, cancellationToken);
        if (userView is null)
            return string.Empty;
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = _key.GetSafeBytes(Encoding.ASCII);
        if (tokenKey is not null && tokenKey.Length == 0)
            return string.Empty;
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(
                new Claim[]
                {
                    new(ClaimTypes.Name, userView.UserName),
                    new(ClaimTypes.Role, userView.RoleName)
                }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        try
        {
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }
}