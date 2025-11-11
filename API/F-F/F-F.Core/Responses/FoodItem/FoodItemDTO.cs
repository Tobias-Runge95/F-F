using F_F.Database.Models;

namespace F_F.Core.Responses.FoodItem;

public class FoodItemDTO
{
    public Guid Id { get; set; }
    public Nutrition Nutrition { get; set; }
    public string Name { get; set; }
}