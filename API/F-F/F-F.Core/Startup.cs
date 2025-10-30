using F_F.Core.IdentityManager;
using F_F.Core.Repositories;
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
            .AddScoped<IAuthenticationManager, AuthenticationManager>();

    }
}