using F_F.Core.Requests.Role;
using F_F.Database;
using F_F.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace F_F.Core.IdentityManager;

public class RoleManager : RoleManager<Role>
{
    private readonly AuthDbContext _context;
    public RoleManager(IRoleStore<Role> store, IEnumerable<IRoleValidator<Role>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<Role>> logger, AuthDbContext context) : base(store, roleValidators, keyNormalizer, errors, logger)
    {
        _context = context;
    }
    
    public async Task<Role?> FindByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _context.Roles.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }
    
    public async Task CreateRole(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var role = new Role() { Id = Guid.NewGuid(), Name = request.Name, NormalizedName = request.Name.ToUpper() };
        await CreateAsync(role);
        await _context.SaveChangesAsync(cancellationToken);
    }
}