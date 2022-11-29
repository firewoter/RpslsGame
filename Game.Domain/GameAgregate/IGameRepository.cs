namespace Game.Domain.GameAggregate;

public interface IGameRepository
{
    public void Save(GameResult gameResult);
}