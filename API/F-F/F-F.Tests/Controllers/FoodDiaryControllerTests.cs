using System.Threading;
using System.Threading.Tasks;
using F_F.API.Controller;
using F_F.Core.Manager.FoodManager;
using F_F.Core.Requests.FoodDiary;
using F_F.Core.Responses.FoodDiary;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace F_F.Tests.Controllers;

public class FoodDiaryControllerTests
{
    private readonly Mock<IFoodDiaryManager> _manager = new();

    [Fact]
    public async Task Create_CallsManager_AndReturnsOk()
    {
        var sut = new FoodDiaryController(_manager.Object);
        var req = new CreateFoodDiaryEntryRequest { UserId = Guid.NewGuid() };
        var result = await sut.Create(req, CancellationToken.None);
        Assert.IsType<OkResult>(result);
        _manager.Verify(m => m.CreateEntry(req, It.IsAny<CancellationToken>()), Times.Once);
    }

    // Intentionally limiting initial tests count; update test can be added later.

    [Fact]
    public async Task Get_CallsManager_AndReturnsOk()
    {
        var sut = new FoodDiaryController(_manager.Object);
        var date = DateTime.UtcNow.Date;
        var userId = Guid.NewGuid();
        _manager.Setup(m => m.GetEntry(date, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FoodDiaryDTO { Id = Guid.NewGuid(), DateTime = date });
        var result = await sut.Get(date, userId, CancellationToken.None) as OkObjectResult;
        Assert.NotNull(result);
        Assert.IsType<FoodDiaryDTO>(result!.Value);
    }

    [Fact]
    public async Task Range_CallsManager_AndReturnsOkList()
    {
        var sut = new FoodDiaryController(_manager.Object);
        var req = new FoodDiaryGetRangeRequest { Start = DateTime.UtcNow.AddDays(-7), End = DateTime.UtcNow, UserId = Guid.NewGuid() };
        _manager.Setup(m => m.GetRange(req, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<FoodDiaryDTO>());
        var result = await sut.GetRange(req, CancellationToken.None) as OkObjectResult;
        Assert.NotNull(result);
        Assert.IsType<List<FoodDiaryDTO>>(result!.Value);
    }
}
