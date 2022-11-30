using Game.Domain.GameAggregate;
using Microsoft.Extensions.Options;

namespace Game.Infrastructure;

public class GameMovesAndRulesRepository : IGameMovesRepository, IGameRulesRepository
{
    private readonly Dictionary<int, GameMoveConfig> _moves;
    private readonly IRandomIntRepository _randomIntRepository;

    public GameMovesAndRulesRepository(IRandomIntRepository randomIntRepository, IOptions<GameConfig> config)
    {
        _randomIntRepository = randomIntRepository;

        _moves = config?.Value?.Moves?.ToDictionary(c => c.Id) 
                 ?? throw new ArgumentException(nameof(config));
    }


    public List<GameMove> GetMoves() => _moves.Values
        .OrderBy(x => x.Id)
        .Select(m => new GameMove(m.Id, m.Name))
        .ToList();

    public GameMove GetMove(int moveId) => 
        _moves.TryGetValue(moveId, out var move) 
            ? new GameMove(move.Id, move.Name) 
            : null;

    public async Task<GameMove> GetRandomMoveAsync()
    {
        var moves = GetMoves();
        var index = int.Abs(await _randomIntRepository.Next()) % moves.Count;
        return moves[index];
    }

    public Dictionary<int, HashSet<int>> GetRules()
    {
        return _moves.ToDictionary(
            x => x.Key, 
            x => x.Value.Beats.ToHashSet());
    }
}