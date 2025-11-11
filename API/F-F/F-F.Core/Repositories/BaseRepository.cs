using System.Linq.Expressions;
using System.Reflection;
using F_F.Database.Mongo;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;

namespace F_F.Core.Repositories;

public interface IBaseRepository<TEntity>
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken);
    Task AddManyAsync(List<TEntity> entities, CancellationToken cancellationToken);
    Task UpdateAsync(FilterDefinition<TEntity> filterDefinition, UpdateDefinition<TEntity> updateDefinition, UpdateOptions updateOptions, CancellationToken cancellationToken);
    Task UpdateManyAsync(FilterDefinition<TEntity> filterDefinition, UpdateDefinition<TEntity> updateDefinition, UpdateOptions updateOptions,
        CancellationToken cancellationToken);
    Task DeleteAsync(FilterDefinition<TEntity> filterDefinition, CancellationToken cancellationToken);
    Task DeleteManyAsync(FilterDefinition<TEntity> filterDefinition, CancellationToken cancellationToken);
    UpdateDefinition<TEntity>? ApplyChanges<T>(T source);
}

public class BaseRepository<TEntity> : IBaseRepository<TEntity>
{
    protected readonly IMongoCollection<TEntity> _collection;

    public BaseRepository(IMongoDbContext mongoDb)
    {
        var type = typeof(TEntity);
         _collection = mongoDb.GetCollection<TEntity>(type.Name);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public async Task AddManyAsync(List<TEntity> entities, CancellationToken cancellationToken)
    {
        await _collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(FilterDefinition<TEntity> filterDefinition, UpdateDefinition<TEntity> updateDefinition, UpdateOptions updateOptions,
        CancellationToken cancellationToken)
    {
        await _collection.UpdateOneAsync(filterDefinition, updateDefinition, updateOptions, cancellationToken);
    }
    
    public async Task UpdateManyAsync(FilterDefinition<TEntity> filterDefinition, UpdateDefinition<TEntity> updateDefinition, UpdateOptions updateOptions,
        CancellationToken cancellationToken)
    {
        await _collection.UpdateManyAsync(filterDefinition, updateDefinition, updateOptions, cancellationToken);
    }

    public async Task DeleteAsync(FilterDefinition<TEntity> filterDefinition, CancellationToken cancellationToken)
    {
        await _collection.DeleteOneAsync(filterDefinition, cancellationToken);
    }

    public async Task DeleteManyAsync(FilterDefinition<TEntity> filterDefinition, CancellationToken cancellationToken)
    {
        await _collection.DeleteManyAsync(filterDefinition, cancellationToken);
    }
    
    public UpdateDefinition<TEntity>? ApplyChanges<T>(T source)
    {
        var updates = new List<UpdateDefinition<TEntity>>();
        var props = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite);

        foreach (var prop in props)
        {
            var newValue = prop.GetValue(source);
            if (newValue != null)
            {
                // Wichtig: Nur Properties mit Werten hinzufügen
                updates.Add(Builders<TEntity>.Update.Set(prop.Name, newValue));
            }
        }

        if (updates.Any())
        {
            return Builders<TEntity>.Update.Combine(updates);
        }

        return null;
    }
}