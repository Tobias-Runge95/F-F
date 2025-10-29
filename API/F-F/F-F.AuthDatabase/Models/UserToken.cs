using Microsoft.AspNetCore.Identity;

namespace WebWorkPlace.Database.Models;

public class UserToken : IdentityUserToken<Guid>
{
    public DateTime Creation { get; set; }
    public DateTime Expiration { get; set; }
}