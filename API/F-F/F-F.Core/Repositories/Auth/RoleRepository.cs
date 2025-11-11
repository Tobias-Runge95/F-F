using F_F.Database;
using F_F.Database.Models;

namespace F_F.Core.Repositories;

public interface IRoleRepository : IBaseAuthRepository<Role>
{
    
}

public class RoleRepository :  BaseAuthRepository<Role>, IRoleRepository
{
    public RoleRepository(AuthDbContext db) : base(db)
    {
    }
}