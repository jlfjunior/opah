using AutoMapper;

namespace Opah.Transaction.API.Business;

public class TransactionProfile : Profile
{
    public TransactionProfile()
    {
        CreateMap<Transaction, TransactionResponse>()
            .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => src.Direction.ToString()));

        CreateMap<CreditTransactionRequest, CreateTransactionCommand>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "Credit"));
        CreateMap<DebitTransactionRequest, CreateTransactionCommand>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "Debit"));
    }
}