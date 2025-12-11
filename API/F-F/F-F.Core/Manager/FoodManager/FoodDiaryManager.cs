using F_F.Core.Repositories.Food;
using F_F.Core.Requests.FoodDiary;
using F_F.Core.Responses.FoodDiary;
using F_F.Database.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using F_F.Core.Responses;

namespace F_F.Core.Manager.FoodManager;

public interface IFoodDiaryManager
{
    Task CreateEntry(CreateFoodDiaryEntryRequest request, CancellationToken cancellationToken);
    Task UpdateEntry(UpdateFoodDiaryEntryRequest request, CancellationToken cancellationToken);
    Task<FoodDiaryDTO> GetEntry(DateTime entryDate, Guid userId, CancellationToken cancellationToken);
    Task<List<FoodDiaryDTO>> GetRange(FoodDiaryGetRangeRequest request, CancellationToken cancellationToken);
    Task<FoodDiaryDTO?> AddFoodItemToMeal(AddFoodItemToMealRequest request, CancellationToken cancellationToken);
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
        if (list.Count < 6)
        {
            var existingDates = list.Select(d => d.Date.Date).ToHashSet();
            for (var date = request.Start.Date; date <= request.End.Date && list.Count < 6; date = date.AddDays(1))
            {
                if (existingDates.Contains(date))
                {
                    continue;
                }

                var newEntry = new FoodDiary
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Date = date,
                    Nutrition = new Nutrition(),
                    Breakfast = new Meal(),
                    Lunch = new Meal(),
                    Dinner = new Meal(),
                    Snacks = new Meal(),
                    Water = 0
                };

                await _repository.AddAsync(newEntry, cancellationToken);
                list.Add(newEntry);
                existingDates.Add(date);
            }
        }
        return list.OrderBy(d => d.Date).Select(entry => entry.ToDTO()).ToList();
    }

    public async Task<FoodDiaryDTO?> AddFoodItemToMeal(AddFoodItemToMealRequest request, CancellationToken cancellationToken)
    {
        var filter = Builders<FoodDiary>.Filter.And(
            Builders<FoodDiary>.Filter.Eq(d => d.Id, request.FoodDiaryId),
            Builders<FoodDiary>.Filter.Eq(d => d.UserId, request.UserId));

        // reuse repository method to fetch by filter
        var diaryList = await _repository.GetTimespanFromUserAsync(filter, cancellationToken);
        var diary = diaryList.FirstOrDefault();
        if (diary is null)
        {
            return null;
        }

        var meal = request.Meal.ToLower() switch
        {
            "breakfast" => diary.Breakfast ??= new Meal(),
            "lunch" => diary.Lunch ??= new Meal(),
            "dinner" => diary.Dinner ??= new Meal(),
            "snacks" => diary.Snacks ??= new Meal(),
            _ => throw new ArgumentException($"Unsupported meal '{request.Meal}'")
        };

        meal.FoodITems ??= new List<FoodItem>();
        meal.FoodITems.Add(request.Item);

        // Recompute nutriments for the meal from all food items
        meal.Nutrition = SumNutriments(meal.FoodITems);

        // Persist only the changed meal
        UpdateDefinition<FoodDiary> update = request.Meal.ToLower() switch
        {
            "breakfast" => Builders<FoodDiary>.Update.Set(d => d.Breakfast, meal),
            "lunch" => Builders<FoodDiary>.Update.Set(d => d.Lunch, meal),
            "dinner" => Builders<FoodDiary>.Update.Set(d => d.Dinner, meal),
            "snacks" => Builders<FoodDiary>.Update.Set(d => d.Snacks, meal),
            _ => throw new ArgumentException($"Unsupported meal '{request.Meal}'")
        };

        await _repository.UpdateAsync(filter, update, null, cancellationToken);

        // Return updated diary (in-memory instance already updated)
        return diary.ToDTO();
    }

    private static Nutriments SumNutriments(IEnumerable<FoodItem> items)
    {
        var result = new Nutriments();

        foreach (var i in items.Where(x => x?.Nutriments != null))
        {
            var n = i.Nutriments;
            result.carbohydrates = (result.carbohydrates ?? 0) + (n.carbohydrates ?? 0);
            result.carbohydrates_100g = (result.carbohydrates_100g ?? 0) + (n.carbohydrates_100g ?? 0);
            result.energy = (result.energy ?? 0) + (n.energy ?? 0);
            result.energy_kcal = (result.energy_kcal ?? 0) + (n.energy_kcal ?? 0);
            result.fat = (result.fat ?? 0) + (n.fat ?? 0);
            result.fat_100g = (result.fat_100g ?? 0) + (n.fat_100g ?? 0);
            result.proteins = (result.proteins ?? 0) + (n.proteins ?? 0);
            result.proteins_100g = (result.proteins_100g ?? 0) + (n.proteins_100g ?? 0);
            result.salt = (result.salt ?? 0) + (n.salt ?? 0);
            result.salt_100g = (result.salt_100g ?? 0) + (n.salt_100g ?? 0);
            result.saturated_fat = (result.saturated_fat ?? 0) + (n.saturated_fat ?? 0);
            result.saturated_fat_100g = (result.saturated_fat_100g ?? 0) + (n.saturated_fat_100g ?? 0);
            result.sugars = (result.sugars ?? 0) + (n.sugars ?? 0);
            result.sugars_100g = (result.sugars_100g ?? 0) + (n.sugars_100g ?? 0);
        }

        return result;
    }
}
