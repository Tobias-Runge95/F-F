using F_F.Core.IdentityManager;
using F_F.Core.Repositories;
using F_F.Core.Repositories.Food;
using F_F.Core.Manager.FoodManager;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace F_F.Core;

public static class Startup
{
    public static IServiceCollection RegisterServices(this IServiceCollection service)
    {
        return service
            .AddScoped<RoleManager>()
            .AddScoped<UserManager>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IRoleRepository, RoleRepository>()
            .AddScoped<IFoodDiaryRepository, FoodDiaryRepository>()
            .AddScoped<IRecipeRepository,RecipeRepository>()
            .AddScoped<IUserDataRepository, UserDataRepository>()
            .AddScoped<IFoodItemRepository, FoodItemRepository>()
            // Food managers
            .AddScoped<IFoodDiaryManager, FoodDiaryManager>()
            .AddScoped<IUserDataManager, UserDataManager>()
            .AddScoped<IFoodItemManager, FoodItemManager>()
            .AddScoped<IAuthenticationManager, AuthenticationManager>();

    }
}
