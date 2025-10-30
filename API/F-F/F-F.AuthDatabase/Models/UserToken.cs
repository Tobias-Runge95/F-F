using Microsoft.AspNetCore.Identity;

namespace F_F.Database.Models;

public class UserToken : IdentityUserToken<Guid>
{
    public DateTime Creation { get; set; }
    public DateTime Expiration { get; set; }
}