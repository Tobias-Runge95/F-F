using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace F_F.Database.Mongo;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var options = MongoOptions.FromConfiguration(configuration);
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            var secretName = configuration["Mongo:ConnectionStringSecretName"]
                            ?? configuration["MongoConnectionStringSecretName"];
            var vaultUri = configuration["AzureKeyVault:VaultUri"]
                           ?? configuration["KeyVaultUri"];
            if (!string.IsNullOrWhiteSpace(secretName) && !string.IsNullOrWhiteSpace(vaultUri))
            {
                try
                {
                    var client = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());
                    var secret = client.GetSecret(secretName);
                    options.ConnectionString = secret.Value.Value;
                }
                catch
                {
                    // fall through; options.ConnectionString stays null -> MongoDbContext will throw a clear error
                }
            }
        }
        services.AddSingleton(options);
        services.AddSingleton<IMongoDbContext, MongoDbContext>();
        return services;
    }
}
