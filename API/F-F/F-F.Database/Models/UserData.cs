using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace F_F.Database.Models;

[BsonIgnoreExtraElements]
public class UserData
{
    public Guid UserId { get; set; }
    public int Age { get; set; }
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public decimal TargetWeight { get; set; }
    public int BodyType { get; set; }
    public Nutrition NutritionGoal { get; set; }
    public List<UserReport> UserReports { get; set; }
}
