using System.Collections.Immutable;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using System.Security.Cryptography;
using F_F.API;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using F_F.Core;
using F_F.Core.Token;
using F_F.Database;
using F_F.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.RegisterKeyVault();
builder.RegisterAuthentication();
builder.Services.AddScoped<IPrincipalProvider, PrincipalProvider>();
// Key Vault signing clients for TokenService
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var vaultUri = (cfg["AzureKeyVault:VaultUri"] ?? cfg["KeyVaultUri"])!;
    return new KeyClient(new Uri(vaultUri), new DefaultAzureCredential());
});
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var vaultUri = (cfg["AzureKeyVault:VaultUri"] ?? cfg["KeyVaultUri"])!;
    var keyName = cfg["Jwt:KeyVaultKeyName"] ?? "JWT";
    var baseKeyId = $"{vaultUri.TrimEnd('/')}/keys/{keyName}";
    return new CryptographyClient(new Uri(baseKeyId), new DefaultAzureCredential());
});
builder.Services.AddScoped<TokenService>();
builder.Services.AddAuthorization();

// Register Auth DB context (PostgreSQL) with connection string from config/Key Vault
var authDbConnectionString = builder.Configuration.GetConnectionString("AuthDatabase")
                           ?? builder.Configuration["ConnectionStrings:AuthDatabase"]
                           ?? builder.Configuration["AuthDatabase:ConnectionString"];

if (string.IsNullOrWhiteSpace(authDbConnectionString))
{
    throw new InvalidOperationException("Connection string 'AuthDatabase' not found. Provide it via Key Vault secret 'ConnectionStrings--AuthDatabase' or configuration.");
}
builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddUserManager<UserManager<User>>()
    .AddRoles<Role>()
    .AddRoleManager<RoleManager<Role>>()
    .AddRoleStore<RoleStore<Role, AuthDbContext, Guid>>()
    .AddUserStore<UserStore<User, IdentityRole<Guid>, AuthDbContext, Guid>>();
builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseNpgsql(authDbConnectionString);
});
builder.Services.AddControllers();
builder.Services.RegisterServices();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Custom middleware to populate PrincipalProvider from the authenticated principal
app.Use(async (context, next) =>
{
    if (context.User?.Identity?.IsAuthenticated == true)
    {
        var principalProvider = context.RequestServices.GetService<IPrincipalProvider>();
        principalProvider?.SetPrincipalUser(context.User);
    }
    await next();
});
app.MapControllers();
app.Run();
