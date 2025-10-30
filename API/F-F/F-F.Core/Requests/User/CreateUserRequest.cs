namespace F_F.Core.Requests.User;

public class CreateUserRequest
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}