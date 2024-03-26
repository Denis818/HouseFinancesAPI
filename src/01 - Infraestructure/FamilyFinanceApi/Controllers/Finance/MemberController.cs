using Application.Interfaces.Services;
using Domain.Dtos.Finance;
using Domain.Models;
using FamilyFinanceApi.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProEventos.API.Controllers.Base;

namespace FamilyFinanceApi.Controllers.Finance
{
    [ApiController]
    [AutorizationFinance]
    [Route("api/[controller]")]
    public class MemberController(IServiceProvider service, 
        IMemberServices MemberServices) : 
        BaseApiController(service)
    {
        [HttpGet]
        public async Task<IEnumerable<Member>> GetAllDespesaAsync() =>
            await MemberServices.GetAllAsync().ToListAsync();

        [HttpGet("{id}")]
        public async Task<Member> GetById(int id)
           => await MemberServices.GetByIdAsync(id);

        [HttpPost]
       // [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Member> Post(MemberDto vendaDto)
            => await MemberServices.InsertAsync(vendaDto);

        [HttpPatch]
      //  [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Member> Put(int id, MemberDto vendaDto)
            => await MemberServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
      //  [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task Delete(int id)
            => await MemberServices.DeleteAsync(id);

    }
}
