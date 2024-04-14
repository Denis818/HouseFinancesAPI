using Application.Interfaces.Services.Finance;
using Domain.Dtos.Membro;
using Domain.Enumeradores;
using Domain.Models.Finance;
using HouseFinancesAPI.Attributes;
using HouseFinancesAPI.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HouseFinancesAPI.Controllers.Finance
{
    [ApiController]
    [AutorizationFinance]
    [Route("api/[controller]")]
    public class MembroController(IServiceProvider service, IMembroAppServices _membroServices)
        : BaseApiController(service)
    {
        [HttpGet]
        public async Task<IEnumerable<Membro>> GetAllDespesaAsync() =>
            await _membroServices.GetAllAsync().ToListAsync();

        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Membro> Post(MembroDto vendaDto) =>
            await _membroServices.InsertAsync(vendaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<Membro> Put(int id, MembroDto vendaDto) =>
            await _membroServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task Delete(int id) => await _membroServices.DeleteAsync(id);
    }
}
