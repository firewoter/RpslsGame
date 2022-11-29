using AutoMapper;
using Game.API.Models;
using Game.Domain.GameAggregate;

namespace Game.API.Configuration;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<GameMove, ChoiceDto>();
        CreateMap<GameResult, PlayResponseDto>()
            .ForMember(dest => dest.Computer, opt => opt.MapFrom(src => src.ComputerMove))
            .ForMember(dest => dest.Player, opt => opt.MapFrom(src => src.PlayerMove))
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.State));
        
        CreateMap<GameState, string>()
            .ConvertUsing(src => src.ToString().ToLower());
    }
}