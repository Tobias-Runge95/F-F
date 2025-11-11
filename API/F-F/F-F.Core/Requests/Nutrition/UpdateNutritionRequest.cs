namespace F_F.Core.Requests.Nutrition;

public class UpdateNutritionRequest
{
    public Guid UserId { get; set; }
    public int Protein { get; set; }
    public int Fats { get; set; }
    public int Carbs { get; set; }
}