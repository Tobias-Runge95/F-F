using F_F.Core.Repositories.Food;
using F_F.Core.Requests.FoodDiary;
using F_F.Core.Responses.FoodDiary;
using F_F.Database.Models;
using MongoDB.Driver;
using System.Linq;

namespace F_F.Core.Manager.FoodManager;

public interface IFoodDiaryManager
{
    Task CreateEntry(CreateFoodDiaryEntryRequest request, CancellationToken cancellationToken);
    Task UpdateEntry(UpdateFoodDiaryEntryRequest request, CancellationToken cancellationToken);
    Task<FoodDiaryDTO> GetEntry(DateTime entryDate, Guid userId, CancellationToken cancellationToken);
    Task<List<FoodDiaryDTO>> GetRange(FoodDiaryGetRangeRequest request, CancellationToken cancellationToken);
}

public class FoodDiaryManager :  IFoodDiaryManager
{
    private readonly IFoodDiaryRepository _repository;

    public FoodDiaryManager(IFoodDiaryRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateEntry(CreateFoodDiaryEntryRequest request, CancellationToken cancellationToken)
    {
        var newEntry = new FoodDiary()
        {
            Id = Guid.NewGuid(),
            Nutrition = request.Nutrition,
            Breakfast = request.Breakfast,
            UserId = request.UserId,
            Date = DateTime.UtcNow,
            Dinner = request.Dinner,
            Lunch = request.Lunch,
            Snacks = request.Snacks,
            Water = request.Water
        };
        await _repository.AddAsync(newEntry, cancellationToken);
    }

    public async Task UpdateEntry(UpdateFoodDiaryEntryRequest request, CancellationToken cancellationToken)
    {
        var updateDefinition = _repository.ApplyChanges(request);
        var filter = Builders<FoodDiary>.Filter.Eq(x => x.Id, request.Id);
        await _repository.UpdateAsync(filter, updateDefinition, null, cancellationToken);
    }

    public async Task<FoodDiaryDTO> GetEntry(DateTime entryDate, Guid userId, CancellationToken cancellationToken)
    {
        var start = entryDate.Date;
        var end = start.AddDays(1).AddTicks(-1);
        var userFilter = Builders<FoodDiary>.Filter.Eq(d => d.UserId, userId);
        var fromFilter = Builders<FoodDiary>.Filter.Gte(d => d.Date, start);
        var toFilter = Builders<FoodDiary>.Filter.Lte(d => d.Date, end);
        var filter = Builders<FoodDiary>.Filter.And(userFilter, fromFilter, toFilter);
        var entries = await _repository.GetTimespanFromUserAsync(filter, cancellationToken);
        var entry = entries.FirstOrDefault();
        if (entry is null)
        {
            return null;
        }

        return entry.ToDTO();
    }

    public async Task<List<FoodDiaryDTO>> GetRange(FoodDiaryGetRangeRequest request, CancellationToken cancellationToken)
    {  
        var userFilter = Builders<FoodDiary>.Filter.Eq(d => d.UserId, request.UserId);
        var fromFilter = Builders<FoodDiary>.Filter.Gte(d => d.Date, request.Start);
        var toFilter = Builders<FoodDiary>.Filter.Lte(d => d.Date, request.End);
        var filter = Builders<FoodDiary>.Filter.And(userFilter, fromFilter, toFilter);
        var list = await _repository.GetTimespanFromUserAsync(filter, cancellationToken);
        return list.Select(entry => entry.ToDTO()).ToList();
    }
}
