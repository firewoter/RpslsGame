namespace Game.Domain.GameAggregate;

public interface IGameRules
{
    public GameState CalculateGameState(int moveId1, int moveId2);
}