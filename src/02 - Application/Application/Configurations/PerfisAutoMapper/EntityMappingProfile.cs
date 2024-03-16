using AutoMapper;
using Domain.Models;
using Domain.Models.Dtos;
using Domain.Models.Finance;

namespace DadosInCached.Configurations.PerfisAutoMapper
{
    public class EntityMappingProfile : Profile
    {
        public EntityMappingProfile()
        {
            CreateMap<Membro, MembroDto>().ReverseMap();
            CreateMap<Despesa, DespesaDto>().ReverseMap();
        }
    }
}
