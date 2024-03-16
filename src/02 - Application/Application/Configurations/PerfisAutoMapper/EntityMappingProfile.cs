using AutoMapper;
using Domain.Models;
using Domain.Models.Dtos;

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
