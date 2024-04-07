using AutoMapper;
using Domain.Dtos.Categoria;
using Domain.Dtos.Finance;
using Domain.Dtos.Membro;
using Domain.Models.Finance;

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
