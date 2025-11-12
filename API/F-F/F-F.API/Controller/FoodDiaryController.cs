using F_F.Core.Manager.FoodManager;
using F_F.Core.Requests.FoodDiary;
using Microsoft.AspNetCore.Mvc;

namespace F_F.API.Controller;

[Controller, Route("food-diary")]
public class FoodDiaryController : ControllerBase
{
    private readonly IFoodDiaryManager _foodDiaryManager;

    public FoodDiaryController(IFoodDiaryManager foodDiaryManager)
    {
        _foodDiaryManager = foodDiaryManager;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateFoodDiaryEntryRequest request, CancellationToken cancellationToken)
    {
        await _foodDiaryManager.CreateEntry(request, cancellationToken);
        return Ok();
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateFoodDiaryEntryRequest request, CancellationToken cancellationToken)
    {
        await _foodDiaryManager.UpdateEntry(request, cancellationToken);
        return Ok();
    }

    [HttpGet("get")]
    public async Task<IActionResult> Get([FromQuery] DateTime entryDate, [FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        var dto = await _foodDiaryManager.GetEntry(entryDate, userId, cancellationToken);
        return Ok(dto);
    }

    [HttpPost("range")]
    public async Task<IActionResult> GetRange([FromBody] FoodDiaryGetRangeRequest request, CancellationToken cancellationToken)
    {
        var list = await _foodDiaryManager.GetRange(request, cancellationToken);
        return Ok(list);
    }
}

