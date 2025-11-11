using F_F.Core.IdentityManager;
using F_F.Core.Requests.Auth;
using Microsoft.AspNetCore.Mvc;

namespace F_F.API.Controller;

[Controller, Route("auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationManager _authenticationManager;

    public AuthenticationController(IAuthenticationManager authenticationManager)
    {
        _authenticationManager = authenticationManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var tokens = await _authenticationManager.LoginAsync(loginRequest, cancellationToken);
        
        return Ok(tokens);
    }

    [HttpPatch("renew")]
    public async Task<IActionResult> RenewToken([FromBody] RenewTokenRequest renewTokenRequest,
        CancellationToken cancellationToken)
    {
        var token = await _authenticationManager.RenewTokenAsync(renewTokenRequest, cancellationToken);
        return Ok(token);
    }

    [HttpDelete("logout/{userId}")]
    public async Task<IActionResult> Logout([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        await _authenticationManager.LogoutAsync(userId, cancellationToken);
        return Ok();
    }
}