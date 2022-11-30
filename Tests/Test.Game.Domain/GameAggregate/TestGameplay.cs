using FluentAssertions;
using Game.Domain.GameAggregate;
using Moq;

namespace Test.Game.Domain;

public class TestGameplay
{
    public static IEnumerable<object[]> GetNullConstructorParameter()
    {
        var gameRulesMock = new Mock<IGameRules>();
        var gameMovesRepositoryMock = new Mock<IGameMovesRepository>();

        yield return new object[] { null, gameMovesRepositoryMock.Object };
        yield return new object[] { gameRulesMock.Object, null };
    }
    
    [Theory]
    [MemberData(nameof(GetNullConstructorParameter))]
    public void Constructor_NullParameter_ThrowsNullArgumentException(IGameRules rules, IGameMovesRepository movesRepository)
    {
        // Arrange
        Action testCode = () => new Gameplay(rules, movesRepository);

        // Act
        var ex = Record.Exception(testCode);
        
        // Assert
        ex.Should().BeOfType<ArgumentNullException>();
    }

    [Fact]
    public async Task PlayAsync_UnknownPlayerMoveId_ThrowsArgumentException()
    {
        // Arrange
        var gameRulesMock = new Mock<IGameRules>();
        var gameMovesRepositoryMock = new Mock<IGameMovesRepository>();
        gameMovesRepositoryMock
            .Setup(x => x.GetMove(It.IsAny<int>()))
            .Returns((GameMove)null);        
        var game = new Gameplay(gameRulesMock.Object, gameMovesRepositoryMock.Object);
        
        // Act
        Func<Task> act = () => game.PlayAsync(0);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }
    
    [Fact]
    public async Task PlayAsync_NullRandomMove_ThrowsInvalidOperationException()
    {
        // Arrange
        var gameRulesMock = new Mock<IGameRules>();
        var gameMovesRepositoryMock = new Mock<IGameMovesRepository>();
        gameMovesRepositoryMock
            .Setup(x => x.GetMove(It.IsAny<int>()))
            .Returns(new GameMove(0, "sample"));        
        gameMovesRepositoryMock
            .Setup(x => x.GetRandomMoveAsync())
            .ReturnsAsync((GameMove)null);   
        var game = new Gameplay(gameRulesMock.Object, gameMovesRepositoryMock.Object);
        
        // Act
        Func<Task> act = () => game.PlayAsync(0);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }
    
    public static IEnumerable<object[]> GetCorrectData()
    {
        var move1 = new GameMove(100, "move1");
        var move2 = new GameMove(200, "move2");
        yield return new object[] { move1, move1, GameState.Tie };
        yield return new object[] { move1, move2, GameState.Win };
        yield return new object[] { move1, move2, GameState.Lose };
    }
    
    [Theory]
    [MemberData(nameof(GetCorrectData))]
    public async Task PlayAsync_CorrectData_ReturnsExpectedValues(GameMove move1, GameMove move2, GameState expectedGameState)
    {
        // Arrange
        var gameRulesMock = new Mock<IGameRules>();
        gameRulesMock
            .Setup(x => x.CalculateGameState(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(expectedGameState);
        
        var gameMovesRepositoryMock = new Mock<IGameMovesRepository>();
        gameMovesRepositoryMock
            .Setup(x => x.GetMove(It.IsAny<int>()))
            .Returns(move1);        
        gameMovesRepositoryMock
            .Setup(x => x.GetRandomMoveAsync())
            .ReturnsAsync(move2);   
        
        var game = new Gameplay(gameRulesMock.Object, gameMovesRepositoryMock.Object);
        
        // Act
        var result = await game.PlayAsync(move1.Id);

        // Assert
        result.Should().NotBeNull();
        result.State.Should().Be(expectedGameState);
        result.PlayerMove.Should().Be(move1.Id);
        result.ComputerMove.Should().Be(move2.Id);
    }
    
    [Fact]
    public async Task PlayAsync_UndefinedGameState_ThrowsInvalidOperationException()
    {
        // Arrange
        var gameRulesMock = new Mock<IGameRules>();
        gameRulesMock.Setup(x => x.CalculateGameState(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(GameState.Undefined);
        
        var gameMovesRepositoryMock = new Mock<IGameMovesRepository>();
        gameMovesRepositoryMock
            .Setup(x => x.GetMove(It.IsAny<int>()))
            .Returns(new GameMove(0, "sample"));        
        gameMovesRepositoryMock
            .Setup(x => x.GetRandomMoveAsync())
            .ReturnsAsync(new GameMove(0, "sample")); 
        
        var game = new Gameplay(gameRulesMock.Object, gameMovesRepositoryMock.Object);
        
        // Act
        Func<Task> act = () => game.PlayAsync(0);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }
}