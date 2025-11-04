using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace F_F.Database.Mongo;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var options = MongoOptions.FromConfiguration(configuration);
        services.AddSingleton(options);
        services.AddSingleton<IMongoDbContext, MongoDbContext>();
        return services;
    }
}

