using F_F.Database.Models;

namespace F_F.Core.Requests.FoodDiary;

public class UpdateFoodDiaryEntryRequest
{
    public Guid Id { get; set; }
    public Database.Models.Nutrition Nutrition { get; set; }
    public Meal Breakfast { get; set; }
    public Meal Lunch { get; set; }
    public Meal Dinner { get; set; }
    public Meal Snacks { get; set; }
    public decimal Water { get; set; }
}