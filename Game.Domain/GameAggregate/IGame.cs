namespace Game.Domain.GameAggregate;

public interface IGame
{
    Task<GameResult> PlayAsync(int playerMoveId);
}