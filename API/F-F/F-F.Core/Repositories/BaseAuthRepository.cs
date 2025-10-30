using F_F.Database;

namespace F_F.Core.Repositories;

public interface IBaseAuthRepository<TEntity>
{
    void Add(TEntity entity, CancellationToken cancellationToken);
    void AddMany(List<TEntity> entities, CancellationToken cancellationToken);
    void Remove(TEntity entity);
    void RemoveMany(List<TEntity> entities, CancellationToken cancellationTokens);
    void Update(TEntity entity);
    void UpdateMany(List<TEntity> entities, CancellationToken cancellationToken);
    void ApplyChanges<T>(TEntity target, T source);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

public class BaseAuthRepository<TEntity> : IBaseAuthRepository<TEntity>
{
    protected readonly AuthDbContext _db;

    public BaseAuthRepository(AuthDbContext db)
    {
        _db = db;
    }

    public void Add(TEntity entity, CancellationToken cancellationToken)
    {
        _db.AddAsync(entity, cancellationToken);
    }

    public void AddMany(List<TEntity> entities, CancellationToken cancellationToken)
    {
        _db.AddRangeAsync(entities, cancellationToken);
    }

    public void Remove(TEntity entity)
    {
        _db.Remove(entity);
    }

    public void RemoveMany(List<TEntity> entities, CancellationToken cancellationToken)
    {
        _db.RemoveRange(entities, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        _db.Update(entity);
    }

    public void UpdateMany(List<TEntity> entities, CancellationToken cancellationToken)
    {
        _db.UpdateRange(entities, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }
    
    public void ApplyChanges<T>(TEntity target, T source)
    {
        var props = typeof(TEntity).GetProperties()
            .Where(p => p.CanWrite && p.CanRead);

        foreach (var prop in props)
        {
            var newValue = prop.GetValue(source);
            if (newValue != null && !Equals(prop.GetValue(target), newValue))
            {
                prop.SetValue(target, newValue);
            }
        }
    }
}