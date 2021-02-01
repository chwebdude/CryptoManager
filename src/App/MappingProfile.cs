using AutoMapper;
using Model.DbModels;
using Model.DTOs;

namespace CryptoManager.App
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Exchange, ExchangeDTO>()
                .ForMember(m => m.ExchangeName, a => a.MapFrom(s => s.ExchangeId.ToString()));

            CreateMap<Fund, FundDTO>();
            CreateMap<CryptoTransaction, InvestmentDTO>();

            CreateMap<FlowNode, FlowNodeDTO>()
                .ForMember(dto => dto.Id,
                    expression => expression.MapFrom(node => node.Id.ToString().Replace("-", string.Empty)));

            CreateMap<FlowLink, FlowLinkDTO>()
                .ForMember(dto => dto.FlowNodeSource,
                    expression => expression.MapFrom(link => link.FlowNodeSource.ToString().Replace("-", string.Empty)))
                .ForMember(dto => dto.FlowNodeTarget,
                    expression => expression.MapFrom(link => link.FlowNodeTarget.ToString().Replace("-", string.Empty)));
        }
    }
}