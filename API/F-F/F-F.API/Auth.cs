using System.Security.Cryptography;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace F_F.API;

public static class Auth
{
    public static WebApplicationBuilder RegisterAuthentication(this WebApplicationBuilder builder)
    {
        string vaultUri = (builder.Configuration["AzureKeyVault:VaultUri"]
                           ?? builder.Configuration["KeyVaultUri"])!;
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var authority = builder.Configuration["Jwt:Authority"];
                var audience = builder.Configuration["Jwt:Audience"];
                var issuer = builder.Configuration["Jwt:Issuer"];
                var keyVaultKeyName = builder.Configuration["Jwt:KeyVaultKeyName"]; // RSA key from Key Vault
                var keyVaultKeyVersion = builder.Configuration["Jwt:KeyVaultKeyVersion"]; // optional

                // Prefer OpenID Connect authority if provided
                if (!string.IsNullOrWhiteSpace(authority))
                {
                    options.Authority = authority;
                    if (!string.IsNullOrWhiteSpace(audience))
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = true,
                            ValidAudience = audience
                        };
                    }

        #if DEBUG
                    options.RequireHttpsMetadata = false;
        #endif
                }
                // Else use RSA public key from Azure Key Vault with rotation support
                else if (!string.IsNullOrWhiteSpace(keyVaultKeyName) && !string.IsNullOrWhiteSpace(vaultUri))
                {
                    var credential = new DefaultAzureCredential();
                    var defaultKeyClient = new KeyClient(new Uri(vaultUri), credential);
                    var cache = new MemoryCache(new MemoryCacheOptions());

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = !string.IsNullOrWhiteSpace(issuer),
                        ValidIssuer = issuer,
                        ValidateAudience = !string.IsNullOrWhiteSpace(audience),
                        ValidAudience = audience,
                        ValidateIssuerSigningKey = true,
                        RequireSignedTokens = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(2),
                        IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                        {
                            var keys = new List<SecurityKey>();

                            // Use 'kid' if provided; otherwise fall back to configured key (latest)
                            var cacheKey = string.IsNullOrWhiteSpace(kid)
                                ? $"kvkey:{keyVaultKeyName}:latest"
                                : kid;

                            if (!cache.TryGetValue(cacheKey, out SecurityKey? cachedKey))
                            {
                                string resolvedVaultUri = vaultUri;
                                string resolvedName = keyVaultKeyName;
                                string? resolvedVersion = keyVaultKeyVersion;

                                // If kid is a Key Vault key id (URI), parse name/version and vault
                                if (!string.IsNullOrWhiteSpace(kid) && Uri.TryCreate(kid, UriKind.Absolute, out var kidUri) && kidUri.Host.Contains(".vault."))
                                {
                                    var segments = kidUri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                                    if (segments.Length >= 2 && segments[0] == "keys")
                                    {
                                        resolvedName = segments[1];
                                        resolvedVersion = segments.Length >= 3 ? segments[2] : null;
                                        resolvedVaultUri = $"{kidUri.Scheme}://{kidUri.Host}/";
                                    }
                                }

                                var client = string.Equals(resolvedVaultUri, vaultUri, StringComparison.OrdinalIgnoreCase)
                                    ? defaultKeyClient
                                    : new KeyClient(new Uri(resolvedVaultUri), credential);

                                var kvKey = string.IsNullOrWhiteSpace(resolvedVersion)
                                    ? client.GetKey(resolvedName).Value
                                    : client.GetKey(resolvedName, resolvedVersion).Value;

                                var jwk = kvKey.Key; // Azure.Security.KeyVault.Keys.JsonWebKey
                                if (jwk.N is null || jwk.E is null)
                                {
                                    throw new InvalidOperationException("The Key Vault RSA key is missing modulus or exponent.");
                                }

                                var rsa = RSA.Create();
                                rsa.ImportParameters(new RSAParameters
                                {
                                    Modulus = jwk.N.ToArray(),
                                    Exponent = jwk.E.ToArray()
                                });

                                var rsaSecurityKey = new RsaSecurityKey(rsa)
                                {
                                    KeyId = kid ?? kvKey.Id.ToString()
                                };

                                cachedKey = rsaSecurityKey;
                                cache.Set(cacheKey, cachedKey, TimeSpan.FromMinutes(30));
                            }

                            if (cachedKey is not null)
                            {
                                keys.Add(cachedKey);
                            }

                            return keys;
                        }
                    };
                }
            });
        return builder;
    }
}