using AutoMapper;
using Opah.Consolidation.Domain;

namespace Opah.Consolidation.Application;

public class TransactionProfile : Profile
{
    public TransactionProfile()
    {
        CreateMap<Transaction, TransactionResponse>()
            .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => src.Direction.ToString()));;
    }
}