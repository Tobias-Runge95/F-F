using System.Security.Claims;
using F_F.Core.Requests.Auth;
using F_F.Core.Responses.Authentication;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Exceptions;
using WebWorkPlace.Core.MediatR.Commands.Authentication;
using WebWorkPlace.Core.Repositories;
using WebWorkPlace.Core.Response;
using WebWorkPlace.Core.Token;
using WebWorkPlace.Database;
using WebWorkPlace.Database.Models;

namespace WebWorkPlace.Core.Identity;

public interface IAuthenticationManager
{
    Task<LoginResponse> LoginAsync(LoginRequest  loginRequest, CancellationToken cancellationToken);
    Task<RenewTokenResponse> RenewTokenAsync(RenewTokenRequest renewTokenRequest, CancellationToken cancellationToken);
    Task LogoutAsync(Guid userId, CancellationToken cancellationToken);
}

public class AuthenticationManager : IAuthenticationManager
{
    private readonly UserManager _userManager;
    private readonly TokenService _tokenService;
    private readonly AuthDbContext _dbContext;

    public AuthenticationManager(UserManager userManager, TokenService tokenService, AuthDbContext dbContext)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _dbContext = dbContext;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserByEmailAsync(loginRequest.Email, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("user not found");
        }
        
        bool isPasswordValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
        if (!isPasswordValid)
        {
            throw new InvalidCredentialsException("Invalid username or password.");
        }
        var claims = new  List<Claim>{new Claim(ClaimTypes.Name, "LoggedIn"),  new Claim(ClaimTypes.Name, "User"),
            new Claim(ClaimTypes.Actor, user.Id.ToString())};
        var token = await _tokenService.GenerateTokensAsync(claims);
        _dbContext.UserTokens.Add(new UserToken{Name = "Base", UserId = user.Id, Value = token.RenewToken, LoginProvider = "Base" });
        await _dbContext.SaveChangesAsync(cancellationToken);
        return token;
    }

    public async Task<RenewTokenResponse> RenewTokenAsync(RenewTokenRequest renewTokenRequest, CancellationToken cancellationToken)
    {
        var dbToken = await _dbContext.UserTokens.FirstOrDefaultAsync(t => t.UserId == renewTokenRequest.UserId &&  t.Name == "Base", cancellationToken: cancellationToken)
                      ??  throw new InvalidTokenException("Invalid token");
        ValidateToken((dbToken as UserToken)!, renewTokenRequest.RenewToken);
        var claims = new  List<Claim>{new Claim(ClaimTypes.Name, "LoggedIn"),  new Claim(ClaimTypes.Name, "User"),
            new Claim(ClaimTypes.Actor, renewTokenRequest.UserId.ToString())};
        var token = await _tokenService.GenerateTokensAsync(claims);
        return new RenewTokenResponse(){AuthToken = token.AuthToken};
    }

    public async Task LogoutAsync(Guid userId, CancellationToken cancellationToken)
    {
        var dbTokens = await _dbContext.UserTokens.Where(t => t.UserId == userId).ToListAsync(cancellationToken);
        if (dbTokens.Count > 0)
        {
            _dbContext.UserTokens.RemoveRange(dbTokens);
            await _dbContext.SaveChangesAsync(cancellationToken); 
        }
    }
    
    private void ValidateToken(UserToken token, string tokenToCheck)
    {
        var now = DateTime.UtcNow;
        if (token.Expiration.CompareTo(now) < 0)
        {
            throw new TokenExpiredException("Token has expired");
        }
        var result = String.Equals(token.Value, tokenToCheck, StringComparison.InvariantCultureIgnoreCase);
        if (!result)
        {
            throw new InvalidTokenException("Invalid token");
        }
    }
}