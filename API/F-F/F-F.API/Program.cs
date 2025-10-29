using System.Collections.Immutable;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Azure.Security.KeyVault.Keys;
using System.Security.Cryptography;
using F_F.API;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using WebWorkPlace.Database;

var builder = WebApplication.CreateBuilder(args);
builder.RegisterKeyVault();
builder.RegisterAuthentication();
builder.Services.AddAuthorization();

// Register Auth DB context (PostgreSQL) with connection string from config/Key Vault
var authDbConnectionString = builder.Configuration.GetConnectionString("AuthDatabase")
                           ?? builder.Configuration["ConnectionStrings:AuthDatabase"]
                           ?? builder.Configuration["AuthDatabase:ConnectionString"];

if (string.IsNullOrWhiteSpace(authDbConnectionString))
{
    throw new InvalidOperationException("Connection string 'AuthDatabase' not found. Provide it via Key Vault secret 'ConnectionStrings--AuthDatabase' or configuration.");
}

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseNpgsql(authDbConnectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Example protected endpoint to verify JWT header is checked
app.MapGet("/secure", (HttpContext ctx) => Results.Ok(new { user = ctx.User.Identity?.Name ?? "anonymous" }))
   .RequireAuthorization();

app.Run();
