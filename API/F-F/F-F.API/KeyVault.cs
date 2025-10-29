using Azure.Identity;

namespace F_F.API;

public static class KeyVault
{
    public static WebApplicationBuilder RegisterKeyVault(this WebApplicationBuilder builder)
    {
        string vaultUri = (builder.Configuration["AzureKeyVault:VaultUri"]
                           ?? builder.Configuration["KeyVaultUri"])!;
        if (string.IsNullOrWhiteSpace(vaultUri))
        {
            var vaultName = builder.Configuration["AzureKeyVault:VaultName"]
                            ?? builder.Configuration["KeyVaultName"];
            if (!string.IsNullOrWhiteSpace(vaultName))
            {
                vaultUri = $"https://{vaultName}.vault.azure.net/";
            }
        }

        if (!string.IsNullOrWhiteSpace(vaultUri))
        {
            var credential = new DefaultAzureCredential();
            builder.Configuration.AddAzureKeyVault(new Uri(vaultUri), credential);
        }  
        
        return builder;
    }
    
}