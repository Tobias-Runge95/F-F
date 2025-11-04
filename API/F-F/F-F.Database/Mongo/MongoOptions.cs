using Microsoft.Extensions.Configuration;

namespace F_F.Database.Mongo;

public class MongoOptions
{
    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }

    public static MongoOptions FromConfiguration(IConfiguration configuration)
    {
        // Prefer section "Mongo"
        var section = configuration.GetSection("Mongo");
        return new MongoOptions
        {
            ConnectionString = section["ConnectionString"] ?? configuration["MongoConnectionString"],
            DatabaseName = section["DatabaseName"] ?? configuration["MongoDatabase"]
        };
    }
}

