namespace F_F.Database.Models;

public class Recipe
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public List<FoodItem> FoodItems { get; set; }
    public Nutrition Nutrition { get; set; }
    public Guid UserId { get; set; }
}