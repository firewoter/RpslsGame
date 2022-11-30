namespace Game.Domain.GameAggregate;

public interface IGameRulesRepository
{
    public Dictionary<int, HashSet<int>> GetRules();
}