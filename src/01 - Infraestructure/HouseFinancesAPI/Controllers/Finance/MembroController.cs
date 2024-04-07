using Domain.Enumeradores;
using HouseFinancesAPI.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HouseFinancesAPI.Controllers.Base;
using Application.Interfaces.Services.Finance;
using Domain.Dtos.Membro;
using Domain.Models.Finance;

namespace HouseFinancesAPI.Controllers.Finance
{
    [ApiController]
    [AutorizationFinance]
    [Route("api/[controller]")]
    public class MembroController(IServiceProvider service,
        IMembroServices _membroServices) :
        BaseApiController(service)
    {
        [HttpGet]
        public async Task<IEnumerable<Membro>> GetAllDespesaAsync() =>
            await _membroServices.GetAllAsync().ToListAsync();

        [HttpGet("{id}")]
        public async Task<Membro> GetById(int id)
           => await _membroServices.GetByIdAsync(id);

        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Membro> Post(MembroDto vendaDto)
            => await _membroServices.InsertAsync(vendaDto);

        [HttpPatch]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Membro> Put(int id, MembroDto vendaDto)
            => await _membroServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task Delete(int id)
            => await _membroServices.DeleteAsync(id);
    }
}
