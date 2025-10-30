using System.Security.Claims;

namespace F_F.Core;

public interface IPrincipalProvider
{
    PrincipalUser? GetPrincipalUser();
    void SetPrincipalUser(ClaimsPrincipal claimsPrincipal);
}

public class PrincipalProvider :  IPrincipalProvider
{
    private PrincipalUser? principalUser;
    public PrincipalUser? GetPrincipalUser() => principalUser; 

    public void SetPrincipalUser(ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
        if (!Guid.TryParse(userId, out var guid))
        {
            throw new Exception("Invalid Actor");
        }
        principalUser = new PrincipalUser { UserId = guid };
    }
}

public class PrincipalUser
{
    public Guid UserId { get; set; }
}
