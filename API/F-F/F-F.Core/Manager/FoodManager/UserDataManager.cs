using System.Collections.Generic;
using F_F.Core.Repositories.Food;
using F_F.Core.Requests.Nutrition;
using F_F.Core.Requests.UserData;
using F_F.Core.Requests.UserReport;
using F_F.Core.Responses.UserData;
using F_F.Database.Models;
using MongoDB.Driver;

namespace F_F.Core.Manager.FoodManager;

public interface IUserDataManager
{
    Task CreateUserData(Guid userId, CancellationToken cancellationToken);
    Task UpdateUserData(UpdateUserDataRequest request, CancellationToken cancellationToken);
    Task DeleteUserData(Guid userId, CancellationToken cancellationToken);
    Task<UserData>  GetUserData(Guid userId, CancellationToken cancellationToken);
    Task<UserDataReport> GetUserDataReport(Guid userId, CancellationToken cancellationToken);
    Task UpdateNutritionTargets(UpdateNutritionRequest request, CancellationToken cancellationToken);
    Task AddUserReport(AddUserReportRequest request, CancellationToken cancellationToken);
}

public class UserDataManager : IUserDataManager
{
    private readonly IUserDataRepository _userDataRepository;

    public UserDataManager(IUserDataRepository userDataRepository)
    {
        _userDataRepository = userDataRepository;
    }

    public async Task CreateUserData(Guid userId, CancellationToken cancellationToken)
    {
        var userData =  new UserData();
        userData.UserId = userId;
        await _userDataRepository.AddAsync(userData, cancellationToken);
    }

    public async Task UpdateUserData(UpdateUserDataRequest request, CancellationToken cancellationToken)
    {
        var filter = Builders<UserData>.Filter.Eq("UserId", request.UserId);
        var updateDefinition = _userDataRepository.ApplyChanges(request);
        await _userDataRepository.UpdateAsync(filter, updateDefinition, null, cancellationToken);
    }

    public async Task DeleteUserData(Guid userId, CancellationToken cancellationToken)
    {
        var filter = Builders<UserData>.Filter.Eq("UserId", userId);
        await _userDataRepository.DeleteAsync(filter, cancellationToken);
    }

    public async Task<UserData> GetUserData(Guid userId, CancellationToken cancellationToken)
    {
        return await _userDataRepository.GetByIdAsync(userId, cancellationToken);
    }

    public async Task<UserDataReport> GetUserDataReport(Guid userId, CancellationToken cancellationToken)
    {
        var userData =  await _userDataRepository.GetAllUserReportsAsync(userId, cancellationToken);
        return Mapper.ToUserDataReport(userData);
    }

    public async Task UpdateNutritionTargets(UpdateNutritionRequest request, CancellationToken cancellationToken)
    {
        var filter = Builders<UserData>.Filter.Eq("UserId", request.UserId);
        var UpdateDefinition = Builders<UserData>.Update.Set(x => x.NutritionGoal, new Nutrition
        {
            Carbs = request.Carbs,
            Fats = request.Fats,
            Protein = request.Protein
        });
        await _userDataRepository.UpdateAsync(filter, UpdateDefinition, null, cancellationToken);
    }

    public async Task AddUserReport(AddUserReportRequest request, CancellationToken cancellationToken)
    {
        var filter = Builders<UserData>.Filter.Eq("_id", request.UserId);
        var report = new UserReport { Date = request.Date, Weight = request.Weight };
        var userData = await _userDataRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (userData is null)
        {
            throw new InvalidOperationException($"UserData entry with id '{request.UserId}' was not found.");
        }

        UpdateDefinition<UserData> update;
        if (userData.UserReports is null || userData.UserReports.Count == 0)
        {
            update = Builders<UserData>.Update.Set(u => u.UserReports, new List<UserReport> { report });
        }
        else
        {
            update = Builders<UserData>.Update.Push(u => u.UserReports, report);
        }

        await _userDataRepository.UpdateAsync(filter, update, new UpdateOptions { IsUpsert = false }, cancellationToken);
    }
}
