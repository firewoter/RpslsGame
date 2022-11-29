using Game.Domain.GameAggregate;

namespace Game.Infrastructure;

public class RandomIntRepository : IRandomIntRepository
{
    private readonly Random _random = new Random();
    public Task<int> Next()
    {
        return Task.FromResult(_random.Next(100));
    }
}