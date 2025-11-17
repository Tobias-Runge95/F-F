using F_F.Database.Models;
using MongoDB.Driver;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace F_F.Database.Mongo;

public interface IMongoDbContext
{
    IMongoDatabase Database { get; }

    IMongoCollection<UserData> UserData { get; }
    IMongoCollection<FoodDiary> FoodDiaries { get; }
    IMongoCollection<FoodItem> FoodItems { get; }
    IMongoCollection<UserReport> UserReports { get; }
    // add more collections as needed

    IMongoCollection<T> GetCollection<T>(string? name = null);
}

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;
    private static bool _guidConfigured;
    private static readonly object GuidLock = new();

    public MongoDbContext(MongoOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
            throw new ArgumentException("Mongo connection string is required", nameof(options.ConnectionString));
        if (string.IsNullOrWhiteSpace(options.DatabaseName))
            throw new ArgumentException("Mongo database name is required", nameof(options.DatabaseName));

        EnsureGuidRepresentation();

        // Ensure GUIDs use the Standard (RFC4122) representation via connection string
        var cs = options.ConnectionString;
        if (!(cs?.Contains("uuidRepresentation=", StringComparison.OrdinalIgnoreCase) ?? false))
        {
            cs += cs!.Contains("?") ? "&uuidRepresentation=standard" : "?uuidRepresentation=standard";
        }
        var clientSettings = MongoClientSettings.FromConnectionString(cs);
        // Try set UUID/Guid representation in a version-tolerant way
        var uuidProp = typeof(MongoClientSettings).GetProperty("UuidRepresentation", BindingFlags.Public | BindingFlags.Instance);
        if (uuidProp != null && uuidProp.CanWrite)
        {
            uuidProp.SetValue(clientSettings, Enum.Parse(uuidProp.PropertyType, "Standard"));
        }
        else
        {
            var guidProp = typeof(MongoClientSettings).GetProperty("GuidRepresentation", BindingFlags.Public | BindingFlags.Instance);
            if (guidProp != null && guidProp.CanWrite)
            {
                guidProp.SetValue(clientSettings, Enum.Parse(guidProp.PropertyType, "Standard"));
            }
        }
        var client = new MongoClient(clientSettings);
        _database = client.GetDatabase(options.DatabaseName);
    }

    private static void EnsureGuidRepresentation()
    {
        if (_guidConfigured)
        {
            return;
        }

        lock (GuidLock)
        {
            if (_guidConfigured)
            {
                return;
            }

            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            _guidConfigured = true;
        }
    }

    public IMongoDatabase Database => _database;

    public IMongoCollection<UserData> UserData => GetCollection<UserData>();
    public IMongoCollection<FoodDiary> FoodDiaries => GetCollection<FoodDiary>();
    public IMongoCollection<FoodItem> FoodItems => GetCollection<FoodItem>();
    public IMongoCollection<UserReport> UserReports => GetCollection<UserReport>();
    // add more collection properties as needed

    public IMongoCollection<T> GetCollection<T>(string? name = null)
    {
        name ??= typeof(T).Name;
        return _database.GetCollection<T>(name);
    }
}
