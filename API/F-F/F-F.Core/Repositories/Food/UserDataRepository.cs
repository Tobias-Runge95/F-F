using F_F.Database.Models;
using F_F.Database.Mongo;
using MongoDB.Driver;
using F_F.Core.Requests.UserReport;

namespace F_F.Core.Repositories.Food;

public interface IUserDataRepository : IBaseRepository<UserData>
{
    Task<UserData> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<UserReport>> GetAllUserReportsAsync(Guid userId, CancellationToken cancellationToken);
}

public class UserDataRepository :  BaseRepository<UserData>, IUserDataRepository
{
    public UserDataRepository(IMongoDbContext mongoDb) : base(mongoDb)
    {
    }


    public async Task<UserData> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var filter = Builders<UserData>.Filter.Eq("_id", userId);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<UserReport>> GetAllUserReportsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var filter = Builders<UserData>.Filter.Eq("_id", userId);
        // First try fetch only the nested reports if possible
        var projection = Builders<UserData>.Projection.Include(u => u.UserReports);
        var projected = await _collection.Find(filter).Project<UserData>(projection).FirstOrDefaultAsync(cancellationToken);
        if (projected?.UserReports != null)
        {
            return projected.UserReports;
        }
        // Fallback: fetch full document
        var full = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return full?.UserReports ?? new List<UserReport>();
    }
}
