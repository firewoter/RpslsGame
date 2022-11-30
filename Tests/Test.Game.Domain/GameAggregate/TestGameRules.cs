using FluentAssertions;
using Game.Domain.GameAggregate;
using Microsoft.VisualStudio.TestPlatform.Common.Exceptions;
using Moq;

namespace Test.Game.Domain;

public class TestGameRules
{
    [Fact]
    public void Constructor_NullParameter_ThrowsArgumentNullException()
    {
        // Arrange
        Action testCode = () => new GameRules(null);

        // Act
        var ex = Record.Exception(testCode);
        
        // Assert
        ex.Should().BeOfType<ArgumentNullException>();
    }

    [Fact]
    public void CalculateGameState_NullRulesDictionary_ThrowsInvalidOperationException()
    {
        // Arrange
        var rulesRepositoryMock = new Mock<IGameRulesRepository>();
        rulesRepositoryMock
            .Setup(x => x.GetRules())
            .Returns((Dictionary<int,HashSet<int>>)null);

        var gameRules = new GameRules(rulesRepositoryMock.Object);
        Action  testCode = () => gameRules.CalculateGameState(1, 2);

        // Act
        var ex = Record.Exception(testCode);

        // Assert
        ex.Should().BeOfType<InvalidOperationException>();
    }
    
    public static IEnumerable<object[]> GetValues()
    {
        yield return new object[] { null, 1, 1, GameState.Tie };
        
        yield return new object[] { new Dictionary<int,HashSet<int>>(), 1, 1, GameState.Tie };
        yield return new object[] { new Dictionary<int,HashSet<int>>(), 1, 2, GameState.Undefined };

        yield return new object[]
        {
            new Dictionary<int, HashSet<int>> { { 1, new HashSet<int> { 2 } } },
            1,
            2,
            GameState.Win
        };

        yield return new object[]
        {
            new Dictionary<int, HashSet<int>> { { 1, new HashSet<int> { 2 } }, { 2, new HashSet<int> { 1 } } },
            1,
            2,
            GameState.Win
        };
        
        yield return new object[]
        {
            new Dictionary<int, HashSet<int>> { { 1, new HashSet<int> { 2 } }, { 2, new HashSet<int> { 1 } } },
            2,
            1,
            GameState.Win
        };
        
        yield return new object[]
        {
            new Dictionary<int, HashSet<int>> { { 2, new HashSet<int> { 1 } } },
            1,
            2,
            GameState.Lose
        };
        
        yield return new object[]
        {
            new Dictionary<int, HashSet<int>> { { 1, new HashSet<int> { 100 } }, { 2, new HashSet<int> { 200 } } },
            1,
            2,
            GameState.Undefined
        };
    }
    
    [Theory]
    [MemberData(nameof(GetValues))]
    public void CalculateGameState_ProvidedValues_ReturnsExpectedResult(
        Dictionary<int,HashSet<int>> rulesDictionary, 
        int moveId1, 
        int moveId2, 
        GameState expectedState)
    {
        // Arrange
        var rulesRepositoryMock = new Mock<IGameRulesRepository>();
        rulesRepositoryMock
            .Setup(x => x.GetRules())
            .Returns(rulesDictionary);

        var gameRules = new GameRules(rulesRepositoryMock.Object);
        Action  testCode = () => gameRules.CalculateGameState(1, 2);

        // Act
        var result = gameRules.CalculateGameState(moveId1, moveId2);

        // Assert
        result.Should().Be(expectedState);

        if (moveId1 == moveId2)
        {
            rulesRepositoryMock.Verify(mock => mock.GetRules(), Times.Never);
        }
        else
        {
            rulesRepositoryMock.Verify(mock => mock.GetRules(), Times.Once);
        }
    }
}