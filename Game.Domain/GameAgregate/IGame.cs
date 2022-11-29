namespace Game.Domain.GameAggregate;

public interface IGame
{
    List<GameMove> GetMoves();
    Task<GameMove> GetRandomMoveAsync();
    Task<GameResult> PlayAsync(int playerMoveId);
}