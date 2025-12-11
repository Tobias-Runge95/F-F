using F_F.Database.Models;

namespace F_F.Core.Requests.FoodDiary;

public class AddFoodItemToMealRequest
{
    public Guid FoodDiaryId { get; set; }
    public Guid UserId { get; set; }
    public string Meal { get; set; } = "";
    public FoodItem Item { get; set; }
}

