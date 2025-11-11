using F_F.Core.Repositories;
using F_F.Core.Requests.User;
using F_F.Core.Responses.User;
using F_F.Database;
using F_F.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace F_F.Core.IdentityManager;

public class UserManager : UserManager<User>
{
    private readonly IUserStore<User> _store;
    private readonly IUserRepository _userRepository;
    private readonly RoleManager _roleManager;
    private readonly AuthDbContext _context;
    public UserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators,
        IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger,
        IUserRepository userRepository, RoleManager roleManager, AuthDbContext context) : base(store, optionsAccessor,
        passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        _userRepository = userRepository;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<User> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = new User() { Id = Guid.NewGuid(),UserName = request.UserName, Email = request.Email };
        var result = await CreateAsync(user, request.Password);
        // await SetSecurityStamp(user, cancellationToken);
        _userRepository.Add(user, cancellationToken);
        var roleResult = await _roleManager.FindByNameAsync("User", cancellationToken);
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
        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<User> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _userRepository.GetUserAsync(userId, cancellationToken) ?? throw new Exception();
    }

    public async Task UpdateUserAsync(UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserAsync(request.UserId, cancellationToken);
        _userRepository.Update(user);
        user.Email = request.Email;
        user.NormalizedUserName = request.UserName.ToUpper();

        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _userRepository.GetUserByEmailAsync(email, cancellationToken);
    }
}