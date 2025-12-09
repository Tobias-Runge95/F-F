using F_F.Core.Exceptions;
using F_F.Core.Repositories.Food;
using F_F.Core.Responses;
using F_F.Core.Responses.FoodItem;
using F_F.Core.Services;
using F_F.Database.Models;

namespace F_F.Core.Manager.FoodManager;

public interface IFoodItemManager
{
    Task<List<SmallFoodItemDTO>> FindByName(string name, CancellationToken cancellationToken);
    Task<OpenFoodFactsDTO> FindById(Guid id, CancellationToken cancellationToken);
    Task<OpenFoodFactsDTO> FindByBarcode(string barcode, CancellationToken cancellationToken);
}

public class FoodItemManager :  IFoodItemManager
{
    private readonly IFoodItemRepository _foodItemRepository;
    private readonly IOpenFoodFactsService _openFoodFactsService;

    public FoodItemManager(IFoodItemRepository foodItemRepository, IOpenFoodFactsService openFoodFactsService)
    {
        _foodItemRepository = foodItemRepository;
        _openFoodFactsService = openFoodFactsService;
    }

    public async Task<List<SmallFoodItemDTO>> FindByName(string name, CancellationToken cancellationToken)
    {
        var list = await _foodItemRepository.SearchByNameAsync(name, cancellationToken);
        return list.Select(x => x.ToSmallDTO()).ToList();
    }

    public async Task<OpenFoodFactsDTO> FindById(Guid id, CancellationToken cancellationToken)
    {
        var item = await _foodItemRepository.GetByIdAsync(id, cancellationToken);
        return item.ToDTO();
    }

    public async Task<OpenFoodFactsDTO> FindByBarcode(string barcode, CancellationToken cancellationToken)
    {
        var item = await _foodItemRepository.GetByBarcodeAsync(barcode, cancellationToken);
        if (item is null)
        {
            // Run the fetch/persist path without the request cancellation token,
            // so it still completes even if the HTTP request is aborted.
            return await FetchAndPersistAsync(barcode);
        }
        return item.ToDTO();
    }

    private async Task<OpenFoodFactsDTO> FetchAndPersistAsync(string barcode)
    {
        var result = await _openFoodFactsService.GetProductAsync(barcode, CancellationToken.None);
        if (result is null)
        {
            throw new NotFoundException();
        }
        var dto = result.ToDTO();
        await CreateItemAsync(dto);
        return dto;
    }

    private async Task CreateItemAsync(OpenFoodFactsDTO item)
    {
        if (item is null)
        {
            return;
        }

        var foodItem = new FoodItem()
        {
            Id = item.Id ?? Guid.NewGuid(),
            Brand = item.Brand,
            Code = item.Code,
            IngredientsText = item.IngredientsText,
            NutritionDataPer = item.NutritionDataPer,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            Nutriments = item.Nutriments,
            Images = item.Images,
            Ingredients = item.Ingredients,
            NutrotionGrades = item.NutrotionGrades
        };
        await _foodItemRepository.AddAsync(foodItem, CancellationToken.None);
    }
}
