using F_F.Core.IdentityManager;
using F_F.Core.Requests.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace F_F.API.Controller;

[Controller, Route("user")]
public class UserController : ControllerBase
{
    private readonly UserManager _userManager;

    public UserController(UserManager userManager)
    {
        _userManager = userManager;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        // var validator = new CreateUserValidator();
        // var results = await validator.ValidateAsync(request);
        // if (!results.IsValid)
        // {
        //     return BadRequest(results.Errors);
        // }
        var user = await _userManager.CreateAsync(request, cancellationToken);
        return Ok(user);
    }

    // [Authorize(Roles = "user, Admin")]
    [HttpPost("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        // var validator = new UpdateUserValidator();
        // var results = await validator.ValidateAsync(request);
        // if (!results.IsValid)
        // {
        //     return BadRequest(results.Errors);
        // }
        await _userManager.UpdateUserAsync(request, cancellationToken);
        return Ok();
    }

    // [Authorize(Roles = "user, Admin")]
    [HttpGet("get")]
    public async Task<IActionResult> GetUser([FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(userId, cancellationToken);
        return Ok(user);
    }

    // [Authorize(Roles = "user, Admin")]
    [HttpDelete("delete/{userId}")]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        await _userManager.DeleteAsync(userId, cancellationToken);
        return Ok();
    }
}