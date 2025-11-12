using F_F.Core.Manager.FoodManager;
using F_F.Core.Requests.Nutrition;
using F_F.Core.Requests.UserData;
using F_F.Core.Requests.UserReport;
using Microsoft.AspNetCore.Mvc;

namespace F_F.API.Controller;

[Controller, Route("user-data")]
public class UserDataController : ControllerBase
{
    private readonly IUserDataManager _userDataManager;

    public UserDataController(IUserDataManager userDataManager)
    {
        _userDataManager = userDataManager;
    }
    
    [HttpPost("create/{userId}")]
    public async Task<IActionResult> Create([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        await _userDataManager.CreateUserData(userId, cancellationToken);
        return Ok();
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateUserDataRequest request, CancellationToken cancellationToken)
    {
        await _userDataManager.UpdateUserData(request, cancellationToken);
        return Ok();
    }

    [HttpDelete("delete/{userId}")]
    public async Task<IActionResult> Delete([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        await _userDataManager.DeleteUserData(userId, cancellationToken);
        return Ok();
    }

    [HttpGet("get/{userId}")]
    public async Task<IActionResult> Get([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var data = await _userDataManager.GetUserData(userId, cancellationToken);
        return Ok(data);
    }

    [HttpGet("report/{userId}")]
    public async Task<IActionResult> GetReport([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var report = await _userDataManager.GetUserDataReport(userId, cancellationToken);
        return Ok(report);
    }

    [HttpPost("nutrition/update")]
    public async Task<IActionResult> UpdateNutrition([FromBody] UpdateNutritionRequest request, CancellationToken cancellationToken)
    {
        await _userDataManager.UpdateNutritionTargets(request, cancellationToken);
        return Ok();
    }

    [HttpPost("report/add")]
    public async Task<IActionResult> AddReport([FromBody] AddUserReportRequest request, CancellationToken cancellationToken)
    {
        await _userDataManager.AddUserReport(request, cancellationToken);
        return Ok();
    }
}

