namespace Game.Domain.GameAggregate;

public interface IGameMovesRepository
{
    public List<GameMove> GetMoves();
    public GameMove GetMove(int moveId);
    public Task<GameMove> GetRandomMoveAsync();
}