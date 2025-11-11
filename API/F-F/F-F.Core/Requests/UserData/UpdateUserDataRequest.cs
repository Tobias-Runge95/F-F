namespace F_F.Core.Requests.UserData;

public class UpdateUserDataRequest
{
    public Guid UserId { get; set; }
    public int Age { get; set; }
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public decimal TargetWeight { get; set; }
    public int BodyType { get; set; }
}