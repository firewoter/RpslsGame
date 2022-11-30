using System.Text.Json;
using Game.Domain.GameAggregate;
using System.Net.Http;

namespace Game.Infrastructure;

public class RandomIntRepository : IRandomIntRepository
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    public RandomIntRepository(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<int> Next()
    {
        var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            "https://codechallenge.boohma.com/random");
        
        var httpClient = _httpClientFactory.CreateClient();
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
        
        
        var result = JsonSerializer.Deserialize<ResponseModel>(await httpResponseMessage.Content.ReadAsStringAsync());

        return result.RandomNumber;
    }
}