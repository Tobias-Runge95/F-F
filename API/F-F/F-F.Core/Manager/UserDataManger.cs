using F_F.Core.Repositories.Food;
using F_F.Core.Responses.UserData;
using F_F.Database.Models;

namespace F_F.Core.Manager;

public interface IUserDataManger
{
    Task<UserDataDTO> GetUserData(Guid userId, CancellationToken cancellationToken);
    Task<List<UserReport>> GetAllUserReportsFromUser(Guid userId, CancellationToken cancellationToken);
}

public class UserDataManger : IUserDataManger
{
    private readonly IUserDataRepository _userDataRepository;

    public UserDataManger(IUserDataRepository userDataRepository)
    {
        _userDataRepository = userDataRepository;
    }

    public async Task<UserDataDTO> GetUserData(Guid userId, CancellationToken cancellationToken)
    {
        var userData = await _userDataRepository.GetByIdAsync(userId, cancellationToken);
        return userData.ToDTO();
    }

    public async Task<List<UserReport>> GetAllUserReportsFromUser(Guid userId, CancellationToken cancellationToken)
    {
        var reports = await _userDataRepository.GetAllUserReportsAsync(userId, cancellationToken);
        return reports ?? new List<UserReport>();
    }
}
