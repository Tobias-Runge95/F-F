using F_F.Database;
using F_F.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace F_F.Core.Repositories;

public interface IUserRepository : IBaseAuthRepository<User>
{
    Task<User?> GetUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
}

public class UserRepository : BaseAuthRepository<User>, IUserRepository
{
    public UserRepository(AuthDbContext db) : base(db)
    {
    }

    public async Task<User?> GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
        {
            user = await _db.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == email.ToUpper());
        }
        
        return user;
    }
}