using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using F_F.Core.Responses.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace F_F.Core.Token;

public class TokenService
{
    private readonly KeyClient _keyClient;
    private readonly CryptographyClient _cryptographyClient;
    private readonly IConfiguration _configuration;

    public TokenService(KeyClient keyClient, CryptographyClient cryptographyClient, IConfiguration configuration)
    {
        _keyClient = keyClient;
        _cryptographyClient = cryptographyClient;
        _configuration = configuration;
    }

    public async Task<LoginResponse> GenerateTokensAsync(IEnumerable<Claim> claims)
    {
        // Key aus Key Vault abrufen
        var keyName = _configuration["Jwt:KeyVaultKeyName"];
        var keyVaultKey = await _keyClient.GetKeyAsync(keyName);
        var dummyKey = new RsaSecurityKey(RSA.Create()) { KeyId = keyVaultKey.Value.Id.ToString() };

        var factory = new AzureKeyVaultCryptoProviderFactory(_cryptographyClient);

        var signingCredentials = new SigningCredentials(dummyKey, SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = factory
        };

        // Standard-Claims sicherstellen
        var now = DateTime.UtcNow;
        var identity = new ClaimsIdentity(claims);

        string? userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? identity.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? identity.FindFirst("sub")?.Value
                          ?? identity.FindFirst(ClaimTypes.Actor)?.Value;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            if (identity.FindFirst(ClaimTypes.NameIdentifier) is null)
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
            if (identity.FindFirst(JwtRegisteredClaimNames.Sub) is null)
                identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, userId));
        }

        if (identity.FindFirst(JwtRegisteredClaimNames.Jti) is null)
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

        // Token-Metadaten aus Konfiguration
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var mins) ? mins : 30;

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = identity,
            NotBefore = now,
            IssuedAt = now,
            Expires = now.AddMinutes(expiresMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = signingCredentials
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        string jwt = tokenHandler.WriteToken(token);
        return new LoginResponse{AuthToken = jwt, RenewToken = RenewToken()};
    }

    private string RenewToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(randomBytes)
            .Replace('+', '-')  // für URL-Sicherheit
            .Replace('/', '_')
            .Replace("=", "");
    }
}

