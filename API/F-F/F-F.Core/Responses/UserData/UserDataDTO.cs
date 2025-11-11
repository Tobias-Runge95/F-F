using F_F.Database.Models;

namespace F_F.Core.Responses.UserData;

public class UserDataDTO
{
    public Guid UserId { get; set; }
    public int Age { get; set; }
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public decimal TargetWeight { get; set; }
    public int BodyType { get; set; }
    public Nutrition NutritionGoal { get; set; }
    public UserReport LatestReport { get; set; }
}