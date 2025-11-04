namespace F_F.Database.Models;

public class FoodDiary
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime DateTime { get; set; }
    public Nutrition Nutrition { get; set; }
    public Meal Breakfast { get; set; }
    public Meal Lunch { get; set; }
    public Meal Dinner { get; set; }
    public Meal Snacks { get; set; }
    public decimal Water { get; set; }
    
}