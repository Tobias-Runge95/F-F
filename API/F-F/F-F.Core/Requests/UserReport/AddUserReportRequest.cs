namespace F_F.Core.Requests.UserReport;

public class AddUserReportRequest
{
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public decimal Weight { get; set; }
}