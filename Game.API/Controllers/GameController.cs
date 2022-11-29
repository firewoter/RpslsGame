using AutoMapper;
using Game.API.Models;
using Game.Domain.GameAggregate;
using Microsoft.AspNetCore.Mvc;

namespace Game.API.Controllers;

[ApiController]
public class GameController : ControllerBase
{
    private readonly IGame _game;
    private readonly IMapper _mapper;
    private readonly ILogger<GameController> _logger;

    public GameController(IGame game, IMapper mapper, ILogger<GameController> logger)
    {
        _game = game;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet("choices")]
    [ProducesResponseType(typeof(List<ChoiceDto>), 200)]
    [Produces("application/json")]
    public List<ChoiceDto> GetChoices()
    {
        var moves = _game.GetMoves();
        return _mapper.Map<List<ChoiceDto>>(moves);
    }

    [HttpGet("choice")]
    [ProducesResponseType(typeof(ChoiceDto), 200)]
    [Produces("application/json")]
    public async Task<ChoiceDto> GetRandomChoice()
    {
        var move = await _game.GetRandomMoveAsync();
        return _mapper.Map<ChoiceDto>(move);
    }

    [HttpPost("play")]
    [ProducesResponseType(typeof(PlayResponseDto), 200)]
    [Produces("application/json")]
    public async Task<ActionResult<PlayResponseDto>> Play(PlayRequestDto request)
    {
        try
        {
            var gameResult = await _game.PlayAsync(request.Player);
            var playResponse = _mapper.Map<PlayResponseDto>(gameResult);
            return playResponse;
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Wrong request: {request}", request);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Wrong request: {request}", request);
            throw;
        }
    }
}