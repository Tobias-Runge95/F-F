using F_F.Database.Models;
using F_F.Database.Mongo;
using MongoDB.Driver;

namespace F_F.Core.Repositories.Food;

public interface IRecipeRepository : IBaseRepository<Recipe>
{
    Task<Recipe> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Recipe> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<List<Recipe>> GetAllFromUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<Recipe>> GetAllPublicAsync(CancellationToken cancellationToken);
}

public class RecipeRepository : BaseRepository<Recipe>, IRecipeRepository
{
    public RecipeRepository(IMongoDbContext mongoDb) : base(mongoDb)
    {
    }

    public async Task<Recipe> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var filter = Builders<Recipe>.Filter.Eq("_id", id);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Recipe> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var filter = Builders<Recipe>.Filter.Eq(x => x.Name, name);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Recipe>> GetAllFromUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var filter = Builders<Recipe>.Filter.Eq(x => x.UserId, userId);
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<List<Recipe>> GetAllPublicAsync(CancellationToken cancellationToken)
    {
        var filter = Builders<Recipe>.Filter.Eq(x => x.Public, true);
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }
}