namespace Game.Infrastructure;

public interface IRandomIntRepository
{
    public Task<int> Next();
}