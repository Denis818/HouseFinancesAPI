using Application.Interfaces.Services;
using Domain.Dtos.Finance;
using Domain.Models;
using FamilyFinanceApi.Attributes;
using FamilyFinanceApi.Extensios.Swagger.ExamplesSwagger;
using FamilyFinanceApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProEventos.API.Controllers.Base;
using Swashbuckle.AspNetCore.Filters;

namespace FamilyFinanceApi.Controllers.Finance
{
    [ApiController]
    [PermissoesFinance]
    [AutorizationFinance]
    [Route("api/[controller]")]
    public class MemberController(IServiceProvider service, 
        IMemberServices MemberServices) : 
        BaseApiController(service)
    {
        [HttpGet]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PageMemberExample))]
        public async Task<IEnumerable<Member>> GetAllDespesaAsync() =>
            await MemberServices.GetAllMembersAsync().ToListAsync();

        [HttpGet("{id}")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MemberExample))]
        public async Task<Member> GetById(int id)
           => await MemberServices.GetByIdAsync(id);

        [HttpPost]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MemberExample))]
        public async Task<Member> Post(MemberDto vendaDto)
            => await MemberServices.InsertAsync(vendaDto);

        [HttpPatch]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MemberExample))]
        public async Task<Member> Put(int id, MemberDto vendaDto)
            => await MemberServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
        public async Task Delete(int id)
            => await MemberServices.DeleteAsync(id);

    }
}
