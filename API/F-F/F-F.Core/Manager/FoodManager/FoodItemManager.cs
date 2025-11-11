using F_F.Core.Repositories.Food;
using F_F.Core.Responses.FoodItem;

namespace F_F.Core.Manager.FoodManager;

public interface IFoodItemManager
{
    Task<List<SmallFoodItemDTO>> FindByName(string name, CancellationToken cancellationToken);
    Task<FoodItemDTO> FindById(Guid id, CancellationToken cancellationToken);
    Task<FoodItemDTO> FindByBarcode(string barcode, CancellationToken cancellationToken);
}

public class FoodItemManager :  IFoodItemManager
{
    private readonly IFoodItemRepository _foodItemRepository;

    public FoodItemManager(IFoodItemRepository foodItemRepository)
    {
        _foodItemRepository = foodItemRepository;
    }

    public async Task<List<SmallFoodItemDTO>> FindByName(string name, CancellationToken cancellationToken)
    {
        var list = await _foodItemRepository.SearchByNameAsync(name, cancellationToken);
        return list.Select(x => x.ToSmallDTO()).ToList();
    }

    public async Task<FoodItemDTO> FindById(Guid id, CancellationToken cancellationToken)
    {
        var item = await _foodItemRepository.GetByIdAsync(id, cancellationToken);
        return item.ToDTO();
    }

    public async Task<FoodItemDTO> FindByBarcode(string barcode, CancellationToken cancellationToken)
    {
        var item = await _foodItemRepository.GetByBarcodeAsync(barcode, cancellationToken);
        return item.ToDTO();
    }
}