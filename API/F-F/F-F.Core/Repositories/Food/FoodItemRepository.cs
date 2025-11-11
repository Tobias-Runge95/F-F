using F_F.Database.Models;
using F_F.Database.Mongo;
using MongoDB.Driver;

namespace F_F.Core.Repositories.Food;

public interface IFoodItemRepository : IBaseRepository<FoodItem>
{
    Task<List<FoodItem>> SearchByNameAsync(string name, CancellationToken cancellationToken);
    Task<FoodItem> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken);
    Task<FoodItem> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}

public class FoodItemRepository :  BaseRepository<FoodItem>, IFoodItemRepository
{
    public FoodItemRepository(IMongoDbContext mongoDb) : base(mongoDb)
    {
    }

    public async Task<List<FoodItem>> SearchByNameAsync(string name, CancellationToken cancellationToken)
    {
        var filter = Builders<FoodItem>.Filter.Eq(x => x.Name, name);
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<FoodItem> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken)
    {
        var filter = Builders<FoodItem>.Filter.Eq(x => x.Barcode, barcode);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FoodItem> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var filter = Builders<FoodItem>.Filter.Eq(x => x.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }
}