using F_F.Database.Models;
using F_F.Database.Mongo;
using MongoDB.Driver;

namespace F_F.Core.Repositories.Food;

public interface IFoodDiaryRepository : IBaseRepository<FoodDiary>
{
    Task<List<FoodDiary>> GetAllFromUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<FoodDiary>> GetTimespanFromUserAsync(FilterDefinition<FoodDiary> filter, CancellationToken cancellationToken);
}

public class FoodDiaryRepository :  BaseRepository<FoodDiary>, IFoodDiaryRepository
{
    public FoodDiaryRepository(IMongoDbContext mongoDb) : base(mongoDb)
    {
    }

    public async Task<List<FoodDiary>> GetAllFromUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var filter = Builders<FoodDiary>.Filter.Eq("UserId", userId);
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<List<FoodDiary>> GetTimespanFromUserAsync(FilterDefinition<FoodDiary> filter, CancellationToken cancellationToken)
    {
        return await _collection
            .Find(filter)
            .SortBy(d => d.Date)
            .ToListAsync(cancellationToken);
    }
}
