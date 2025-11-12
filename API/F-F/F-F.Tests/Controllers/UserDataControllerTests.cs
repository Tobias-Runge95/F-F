using System.Threading;
using System.Threading.Tasks;
using F_F.API.Controller;
using F_F.Core.Manager.FoodManager;
using F_F.Core.Requests.Nutrition;
using F_F.Core.Requests.UserData;
using F_F.Core.Requests.UserReport;
using F_F.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace F_F.Tests.Controllers;

public class UserDataControllerTests
{
    private readonly Mock<IUserDataManager> _manager = new();

    [Fact]
    public async Task Create_CallsManager_AndReturnsOk()
    {
        var sut = new UserDataController(_manager.Object);
        var result = await sut.Create(Guid.NewGuid(), CancellationToken.None);
        Assert.IsType<OkResult>(result);
        _manager.Verify(m => m.CreateUserData(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Update_CallsManager_AndReturnsOk()
    {
        var sut = new UserDataController(_manager.Object);
        var req = new UpdateUserDataRequest { UserId = Guid.NewGuid() };
        var result = await sut.Update(req, CancellationToken.None);
        Assert.IsType<OkResult>(result);
        _manager.Verify(m => m.UpdateUserData(req, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_CallsManager_AndReturnsOk()
    {
        var sut = new UserDataController(_manager.Object);
        var id = Guid.NewGuid();
        var result = await sut.Delete(id, CancellationToken.None);
        Assert.IsType<OkResult>(result);
        _manager.Verify(m => m.DeleteUserData(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Get_ReturnsOkWithPayload()
    {
        var sut = new UserDataController(_manager.Object);
        var id = Guid.NewGuid();
        _manager.Setup(m => m.GetUserData(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserData { UserId = id });
        var action = await sut.Get(id, CancellationToken.None) as OkObjectResult;
        Assert.NotNull(action);
        Assert.IsType<UserData>(action!.Value);
    }

    // Intentionally limiting initial tests count; more can be added later.
}
