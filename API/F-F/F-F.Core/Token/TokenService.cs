using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using F_F.Core.Responses.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace WebWorkPlace.Core.Token;

public class TokenService
{
    private readonly KeyClient _keyClient;
    private readonly CryptographyClient _cryptographyClient;

    public TokenService(KeyClient keyClient, CryptographyClient cryptographyClient)
    {
        _keyClient = keyClient;
        _cryptographyClient = cryptographyClient;
    }

    public async Task<LoginResponse> GenerateTokensAsync(IEnumerable<Claim> claims)
    {
        // Schlüssel aus Key Vault abrufen
        var keyVaultKey = await _keyClient.GetKeyAsync("RollplayHelper");
        var dummyKey = new RsaSecurityKey(RSA.Create()) { KeyId = keyVaultKey.Value.Id.ToString() };

        var factory = new AzureKeyVaultCryptoProviderFactory(_cryptographyClient);

        var signingCredentials = new SigningCredentials(dummyKey, SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = factory
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(30),
            Issuer = "IC",
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
            .Replace('/', '_')  // "
            .Replace("=", "");
    }
}