namespace F_F.Database.Models;

public class FoodItem
{
    public Guid Id { get; set; }
    public Nutrition Nutrition { get; set; }
    public string? Barcode { get; set; }
    public string Name { get; set; }
}