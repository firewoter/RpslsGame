using Microsoft.Extensions.Options;

namespace Game.Domain.GameAggregate;

public class Game : IGame
{
    private readonly IGameRules _gameRules;
    private readonly IGameMovesRepository _movesRepository;
    
    public Game(IGameRules gameRules, IGameMovesRepository movesRepository)
    {
        _gameRules = gameRules
                     ?? throw new ArgumentNullException(nameof(gameRules));
        
        _movesRepository = movesRepository
                           ?? throw new ArgumentNullException(nameof(movesRepository));
    }

    public async Task<GameResult> PlayAsync(int playerMoveId)
    {
        var playerMove = _movesRepository.GetMove(playerMoveId) 
                         ?? throw new ArgumentException(nameof(playerMoveId));

        var computerMove = await _movesRepository.GetRandomMoveAsync() 
                           ?? throw new InvalidOperationException(nameof(_movesRepository.GetRandomMoveAsync));

        var state = _gameRules.CalculateGameState(playerMove.Id, computerMove.Id);

        return new GameResult(state, playerMoveId, computerMove.Id);
    }
}