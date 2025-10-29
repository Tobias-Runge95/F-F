using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebWorkPlace.Core.MediatR.Commands.User;
using WebWorkPlace.Core.Repositories;
using WebWorkPlace.Database;
using WebWorkPlace.Database.Models;

namespace WebWorkPlace.Core.Identity;

public class UserManager : UserManager<User>
{
    private readonly IUserStore<User> _store;
    private readonly IUserRepository _userRepository;
    private readonly RoleManager _roleManager;
    private readonly ApplicationDbContext _context;
    public UserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators,
        IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger,
        IUserRepository userRepository, RoleManager roleManager, ApplicationDbContext context) : base(store, optionsAccessor,
        passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        _userRepository = userRepository;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<User> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = new User() { Id = Guid.NewGuid(),UserName = $"{request.FirstName}, {request.LastName}", Email = request.Email };
        var result = await CreateAsync(user, request.Password);
        await SetSecurityStamp(user, cancellationToken);
        _userRepository.Add(user);
        var roleResult = await _roleManager.FindByNameAsync("user", cancellationToken);
        if (roleResult is not null)
        {
            _context.UserRoles.Add(new UserRole { RoleId = roleResult.Id, UserId = user.Id });
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserAsync(userId, cancellationToken);
        _userRepository.Remove(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task<User> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _userRepository.GetUserAsync(userId, cancellationToken) ?? throw new Exception();
    }

    public async Task UpdateUserAsync(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserAsync(request.UserId, cancellationToken);
        _userRepository.Update(user);
        user.UserName = $"{request.FirstName}, {request.LastName}";
        user.NormalizedUserName = $"{request.FirstName} {request.LastName}".ToUpper();

        await _userRepository.SaveChangesAsync();
    }

    public async Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _userRepository.GetUserByEmailAsync(email, cancellationToken);
    }

    private async Task SetSecurityStamp(User user, CancellationToken cancellationToken)
    {
        var securityStampStore = _store as IUserSecurityStampStore<User>;
        if (securityStampStore is null)
        {
            throw new NullReferenceException();
        }
        await securityStampStore.SetSecurityStampAsync(user, Guid.NewGuid().ToString(), cancellationToken);
    }
}