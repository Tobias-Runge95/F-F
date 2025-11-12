using F_F.Core.Manager.FoodManager;
using Microsoft.AspNetCore.Mvc;

namespace F_F.API.Controller;

[Controller, Route("food-item")]
public class FoodItemController : ControllerBase
{
    private readonly IFoodItemManager _foodItemManager;

    public FoodItemController(IFoodItemManager foodItemManager)
    {
        _foodItemManager = foodItemManager;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByName([FromQuery] string name, CancellationToken cancellationToken)
    {
        var list = await _foodItemManager.FindByName(name, cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var item = await _foodItemManager.FindById(id, cancellationToken);
        return Ok(item);
    }

    [HttpGet("barcode/{barcode}")]
    public async Task<IActionResult> GetByBarcode([FromRoute] string barcode, CancellationToken cancellationToken)
    {
        var item = await _foodItemManager.FindByBarcode(barcode, cancellationToken);
        return Ok(item);
    }
}

