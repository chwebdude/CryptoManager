using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Model.DbModels;
using Model.DTOs;

namespace CryptoManager
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Exchange, ExchangeDto>()
                .ForMember(m => m.ExchangeName, a => a.MapFrom(s => s.ExchangeId.ToString()));

            CreateMap<Fund, FundDTO>();
        }
    }
}
