using System.Collections;
using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Game.API.Models;
using Moq;
using Test.Game.API.Helpers;
using Xunit;

namespace Test.Game.API.Controllers;

public class TestGameController : IClassFixture<WebAppFactory>
{
    private readonly HttpClient _httpClient;
    private readonly WebAppFactory _webAppFactory;

    public TestGameController(WebAppFactory webAppFactory)
    {
        _webAppFactory = webAppFactory;
        _httpClient = webAppFactory.CreateClient();
    }

    [Fact]
    public async Task GetChoices_ShouldReturnChoices()
    {
        const string url = "choices";
        
        var response = await _httpClient.GetAsync(url);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = JsonSerializer.Deserialize<List<ChoiceDto>>(await response.Content.ReadAsStringAsync());

        result.Count.Should().Be(5);
        result.Select(x => x.Id).Distinct().Should().HaveCount(5);
    }
    
    public static IEnumerable<object[]> GetNumbers()
    {
        const int min = -1;
        const int max = 11;
        for (int i = min; i <= max; i++)
        {
            yield return new object[] { i };
        }
    }
    
    [Theory]
    [MemberData(nameof(GetNumbers))]
    public async Task GetRandomChoiceAsync_ShouldReturnRandomChoice(int randomNumber)
    {
        const string url = "choice";
        var choices = new List<ChoiceDto>
        {
            new ChoiceDto() { Id = 1, Name = "rock" },
            new ChoiceDto() { Id = 2, Name = "paper" },
            new ChoiceDto() { Id = 3, Name = "scissors" },
            new ChoiceDto() { Id = 4, Name = "lizard" },
            new ChoiceDto() { Id = 5, Name = "spock" }
        };

        _webAppFactory.RandomIntRepositoryMock.Setup(x => x.Next()).ReturnsAsync(randomNumber);
        
        var response = await _httpClient.GetAsync(url);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = JsonSerializer.Deserialize<ChoiceDto>(await response.Content.ReadAsStringAsync());

        choices.Should().Contain(x => x.Id == result.Id && x.Name == result.Name);
    }

    public static IEnumerable<object[]> GetPlayerChoicesAndRandomNumbers()
    {
        const int randomMin = -1;
        const int randomMax = 11;
        int[] playerChoices = new[] { 1, 2, 3, 4, 5 };

        foreach (var playerChoice in playerChoices)
        {
            for (int i = randomMin; i <= randomMax; i++)
            {
                yield return new object[] { playerChoice, i };
            }
        }
    }

    [Theory]
    [MemberData(nameof(GetPlayerChoicesAndRandomNumbers))]
    public async Task Play_CorrectData_ShouldReturnOk(int playerChoice, int randomNumber)
    {
        const string url = "play";
        var rulesDictionary = new Dictionary<int, int[]>
        {
            { 1, new[] { 3, 4 } },
            { 2, new[] { 1, 5 } },
            { 3, new[] { 2, 4 } },
            { 4, new[] { 2, 5 } },
            { 5, new[] { 1, 3 } }
        };

        _webAppFactory.RandomIntRepositoryMock.Setup(x => x.Next()).ReturnsAsync(randomNumber);

        var body = new PlayRequestDto() { Player = playerChoice };
        var bodyJson = JsonSerializer.Serialize(body);
        var stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(url, stringContent);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = JsonSerializer.Deserialize<PlayResponseDto>(await response.Content.ReadAsStringAsync());

        result.Player.Should().Be(playerChoice);
        result.Computer.Should().BeInRange(1, 5);

        var expectedState = result.Player == result.Computer
            ? "tie"
            : rulesDictionary[result.Player].Contains(result.Computer)
                ? "win"
                : "lose";
        
        result.Results.Should().Be(expectedState);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task Play_IncorrectPlayerChoice_ShouldReturnBadRequest(int playerChoice)
    {
        const string url = "play";
        
        var body = new PlayRequestDto() { Player = playerChoice };
        var bodyJson = JsonSerializer.Serialize(body);
        var stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(url, stringContent);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
