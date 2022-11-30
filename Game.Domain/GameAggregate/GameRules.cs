namespace Game.Domain.GameAggregate;

public class GameRules : IGameRules
{
    private readonly IGameRulesRepository _rulesRepository;

    public GameRules(IGameRulesRepository _rulesRepository)
    {
        this._rulesRepository = _rulesRepository;
    }

    public GameState CalculateGameState(int moveId1, int moveId2)
    {
        if (moveId1 == null) throw new ArgumentNullException(nameof(moveId1));
        if (moveId2 == null) throw new ArgumentNullException(nameof(moveId2));

        var rules = _rulesRepository.GetRules() 
                    ?? throw new InvalidOperationException(nameof(_rulesRepository.GetRules));

        if (moveId1 == moveId2)
            return GameState.Tie;

        if (rules.TryGetValue(moveId1, out var beates1) && beates1.Contains(moveId2))
            return GameState.Win;
        
        if (rules.TryGetValue(moveId2, out var beates2) && beates2.Contains(moveId1))
            return GameState.Lose;

        return GameState.Undefined;
    }
}