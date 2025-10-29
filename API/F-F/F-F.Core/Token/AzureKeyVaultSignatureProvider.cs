using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace WebWorkPlace.Core.Token;

public class AzureKeyVaultSignatureProvider : SignatureProvider
{
    private readonly CryptographyClient _cryptoClient;
    private readonly SignatureAlgorithm _signatureAlgorithm;

    public AzureKeyVaultSignatureProvider(CryptographyClient cryptoClient, SecurityKey key, string algorithm, bool willCreateSignatures)
        : base(key, algorithm)
    {
        _cryptoClient = cryptoClient;
        _signatureAlgorithm = SignatureAlgorithm.RS256;
    }

    public override byte[] Sign(byte[] input)
    {
        var result = _cryptoClient.SignData(_signatureAlgorithm, input);
        return result.Signature;
    }

    public override bool Verify(byte[] input, byte[] signature)
        => throw new NotImplementedException();

    protected override void Dispose(bool disposing) { }
}