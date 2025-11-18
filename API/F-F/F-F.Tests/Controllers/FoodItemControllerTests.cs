using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using F_F.API.Controller;
using F_F.Core.Manager.FoodManager;
using F_F.Core.Responses;
using F_F.Core.Responses.FoodItem;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace F_F.Tests.Controllers;

public class FoodItemControllerTests
{
    private readonly Mock<IFoodItemManager> _manager = new();

    [Fact]
    public async Task SearchByName_ReturnsOkWithList()
    {
        var sut = new FoodItemController(_manager.Object);
        _manager.Setup(m => m.FindByName("apple", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<SmallFoodItemDTO>());
        var result = await sut.SearchByName("apple", CancellationToken.None) as OkObjectResult;
        Assert.NotNull(result);
        Assert.IsType<List<SmallFoodItemDTO>>(result!.Value);
    }

    [Fact]
    public async Task GetById_ReturnsOkWithItem()
    {
        var sut = new FoodItemController(_manager.Object);
        var id = Guid.NewGuid();
        _manager.Setup(m => m.FindById(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new OpenFoodFactsDTO { Id = id, ProductName = "Test" });
        var result = await sut.GetById(id, CancellationToken.None) as OkObjectResult;
        Assert.NotNull(result);
        Assert.IsType<OpenFoodFactsDTO>(result!.Value);
    }

    [Fact]
    public async Task GetByBarcode_ReturnsOkWithItem()
    {
        var sut = new FoodItemController(_manager.Object);
        _manager.Setup(m => m.FindByBarcode("123", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new OpenFoodFactsDTO { Id = Guid.NewGuid(), ProductName = "Test" });
        var result = await sut.GetByBarcode("123", CancellationToken.None) as OkObjectResult;
        Assert.NotNull(result);
        Assert.IsType<OpenFoodFactsDTO>(result!.Value);
    }
}
