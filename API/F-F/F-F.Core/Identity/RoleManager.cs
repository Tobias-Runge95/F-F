using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebWorkPlace.Core.MediatR.Commands.Role;
using WebWorkPlace.Database;
using WebWorkPlace.Database.Models;

namespace WebWorkPlace.Core.Identity;

public class RoleManager : RoleManager<Role>
{
    private readonly ApplicationDbContext _context;
    public RoleManager(IRoleStore<Role> store, IEnumerable<IRoleValidator<Role>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<Role>> logger, ApplicationDbContext context) : base(store, roleValidators, keyNormalizer, errors, logger)
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