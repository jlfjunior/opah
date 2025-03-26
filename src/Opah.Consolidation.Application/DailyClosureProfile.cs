using AutoMapper;
using Opah.Consolidation.Domain;

namespace Opah.Consolidation.Application;

public class DailyClosureProfile : Profile
{
    public DailyClosureProfile()
    {
        CreateMap<DailyClosure, DailyClosureResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == DailyClosureStatus.Closed ? "Closed" : "Open"));
    }
}