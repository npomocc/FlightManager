using AutoMapper;
using Flight.Common;
using Flight.Contracts;
using Flight.Data;
using Flight.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flight.Infrastructure;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;
    public AuthService(IUserRepository userRepository,
        IRoleRepository roleRepository,
        ILogger<AuthService> logger, 
        IMapper mapper)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<bool> AddUser(CreateUserView createView, CancellationToken cancellationToken)
    {
        await _userRepository.Insert(_mapper.Map<User>(createView), cancellationToken);
        return true;
    }

    public async Task<UserView?> Auth(LoginView loginView, CancellationToken cancellationToken)
    {
        try
        {
            var hashedPassword = loginView.UserName.Hash(loginView.Password);
            var user = await _userRepository.Auth(loginView.UserName, hashedPassword, cancellationToken);
            if (user is null) return null;
            var role = await _roleRepository.GetById(user.RoleId, cancellationToken);
            if (role is null) return null;
            var userView = _mapper.Map<UserView>(user);
            userView.RoleName = role.Code;
            return userView;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "fail on auth");
            return null;
        }
    }

    public async Task<bool> EditUser(UserView userView, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetById(userView.Id, cancellationToken);
            if (user is null) return false;
            user.UserName = userView.UserName;
            user.Password = userView.Password;
            await _userRepository.Update(user, cancellationToken);
            return true;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "fail on user update");
            return false;
        }
    }

    public async Task<IEnumerable<UserView>> List()
    {
        try
        {
            var result = await _userRepository
                .Get()
                .ToListAsync();
            return _mapper.Map<IEnumerable<UserView>>(result);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "fail on get user list");
            return Enumerable.Empty<UserView>();
        }
    }
}