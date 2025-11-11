namespace F_F.Core.Requests.FoodDiary;

public class FoodDiaryGetRangeRequest
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public Guid UserId { get; set; }
}