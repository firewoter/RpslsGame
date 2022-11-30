namespace Game.Domain.GameAggregate;

public record GameResult(
    GameState State, 
    int PlayerMove, 
    int ComputerMove);