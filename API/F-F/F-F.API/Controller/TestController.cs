using F_F.Core.IdentityManager;
using F_F.Core.Requests.Role;
using Microsoft.AspNetCore.Mvc;

namespace F_F.API.Controller;

[Controller, Route("test")]
public class TestController : ControllerBase
{
    private readonly RoleManager _manager;

    public TestController(RoleManager manager)
    {
        _manager = manager;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        await _manager.CreateRole(request, cancellationToken);
        return Ok();
    }
}