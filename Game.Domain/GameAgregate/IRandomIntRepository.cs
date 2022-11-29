namespace Game.Domain.GameAggregate;

public interface IRandomIntRepository
{
    public Task<int> Next();
}