namespace F_F.Database.Models;

public class Meal
{
    public List<FoodItem> FoodITems { get; set; }
    public Nutrition Nutrition { get; set; }
}