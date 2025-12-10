using F_F.Core.Exceptions;
using F_F.Core.Responses;
using RestSharp;

namespace F_F.Core.Services;

public interface IOpenFoodFactsService
{
    Task<OpenFoodFacts?> GetProductAsync(string barcode);
}

public class OpenFoodFactsService : IOpenFoodFactsService
{
    private readonly RestClient _client;

    public OpenFoodFactsService()
    {
        var options = new RestClientOptions("https://world.openfoodfacts.org/api/");
        _client = new RestClient(options);
    }

    public async Task<OpenFoodFacts?> GetProductAsync(string barcode)
    {
        var request = new RestRequest($"v2/product/{barcode}");
        try
        {
            var result = await _client.GetAsync<OpenFoodFacts>(request);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
