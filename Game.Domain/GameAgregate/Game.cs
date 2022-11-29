using Microsoft.Extensions.Options;

namespace Game.Domain.GameAggregate;

public class Game : IGame
{
    private readonly IRandomIntRepository _randomIntRepository;
    private readonly Dictionary<int, GameMoveConfig> _moves;
    
    public Game(IRandomIntRepository randomIntRepository, IOptions<GameConfig> config)
    {
        _randomIntRepository = randomIntRepository 
                               ?? throw new ArgumentNullException(nameof(randomIntRepository));
        
        _moves = config?.Value?.Moves?.ToDictionary(c => c.Id) 
                 ?? throw new ArgumentException(nameof(config));
    }


    public List<GameMove> GetMoves() => _moves.Values
        .OrderBy(x => x.Id)
        .Select(m => new GameMove(m.Id, m.Name))
        .ToList();

    public async Task<GameMove> GetRandomMoveAsync()
    {
        var moves = GetMoves();
        var index = int.Abs(await _randomIntRepository.Next()) % moves.Count;
        return moves[index];
    }

    public async Task<GameResult> PlayAsync(int playerMoveId)
    {
        if (!_moves.TryGetValue(playerMoveId, out var move))
        {
            throw new ArgumentException(nameof(playerMoveId));
        }

        var computerMove = await GetRandomMoveAsync();

        var state = Compare(playerMoveId, computerMove.Id);

        return new GameResult(state, playerMoveId, computerMove.Id);
    }

    private GameState Compare(int moveId1, int moveId2)
    {
        if (moveId1 == moveId2)
        {
            return GameState.Tie;
        }

        if (_moves.TryGetValue(moveId1, out var move1) && move1.Beats.Contains(moveId2))
        {
            return GameState.Win;
        }
        
        if (_moves.TryGetValue(moveId2, out var move2) && move2.Beats.Contains(moveId1))
        {
            return GameState.Lose;
        }

        return GameState.Undefined;
    }
}

public enum GameState
{
    Undefined,
    Win,
    Lose,
    Tie
}

public record GameResult(
    GameState State, 
    int PlayerMove, 
    int ComputerMove);

public record GameMove (
    int Id, 
    string Name);