using Domain.Enumeradores;
using Domain.Models;
using HouseFinancesAPI.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HouseFinancesAPI.Controllers.Base;
using Application.Interfaces.Services.Finance;
using Domain.Dtos.Membro;

namespace HouseFinancesAPI.Controllers.Finance
{
    [ApiController]
    [AutorizationFinance]
    [Route("api/[controller]")]
    public class MembroController(IServiceProvider service,
        IMembroServices MemberServices) :
        BaseApiController(service)
    {
        [HttpGet]
        public async Task<IEnumerable<Membro>> GetAllDespesaAsync() =>
            await MemberServices.GetAllAsync().ToListAsync();

        [HttpGet("{id}")]
        public async Task<Membro> GetById(int id)
           => await MemberServices.GetByIdAsync(id);

        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Membro> Post(MembroDto vendaDto)
            => await MemberServices.InsertAsync(vendaDto);

        [HttpPatch]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Membro> Put(int id, MembroDto vendaDto)
            => await MemberServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task Delete(int id)
            => await MemberServices.DeleteAsync(id);
    }
}
