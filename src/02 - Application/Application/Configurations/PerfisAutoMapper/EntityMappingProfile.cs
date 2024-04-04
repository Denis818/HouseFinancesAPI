using AutoMapper;
using Domain.Dtos.Finance;
using Domain.Dtos.Finance.Records;
using Domain.Models;

namespace DadosInCached.Configurations.PerfisAutoMapper
{
    public class EntityMappingProfile : Profile
    {
        public EntityMappingProfile()
        {
            CreateMap<Membro, MembroDto>().ReverseMap();
            CreateMap<Despesa, DespesaDto>().ReverseMap();
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
        }
    }
}
