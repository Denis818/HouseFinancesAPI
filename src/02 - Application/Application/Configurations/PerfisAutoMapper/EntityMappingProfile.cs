using AutoMapper;
using Domain.Dtos.Finance;
using Domain.Models.Finance;

namespace DadosInCached.Configurations.PerfisAutoMapper
{
    public class EntityMappingProfile : Profile
    {
        public EntityMappingProfile()
        {
            CreateMap<Member, MemberDto>().ReverseMap();
            CreateMap<Despesa, DespesaDto>().ReverseMap();
        }
    }
}
