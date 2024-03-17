using Application.Interfaces.Services;
using Domain.Dtos.Finance;
using Domain.Models.Finance;
using FamilyFinanceApi.Attributes;
using FamilyFinanceApi.Extensios.Swagger.ExamplesSwagger;
using FamilyFinanceApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Controllers.Base;
using Swashbuckle.AspNetCore.Filters;

namespace FamilyFinanceApi.Controllers.Finance
{
    [ApiController]
    [PermissoesFinance]
    [AutorizationFinance]
    [Route("api/[controller]")]
    public class DespesaController(IServiceProvider service, 
        IDespesaServices DespesaServices) : 
        BaseApiController(service)
    {
        [HttpGet]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PageDespesaExample))]
        public async Task<PagedResult<Despesa>> GetAllDespesaAsync(int paginaAtual = 1, int itensPorPagina = 10) =>
            await DespesaServices.GetAllDespesaAsync(paginaAtual, itensPorPagina);


        [HttpGet("{id}")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DespesaExample))]
        public async Task<Despesa> GetById(int id)
           => await DespesaServices.GetByIdAsync(id);

        [HttpPost]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DespesaExample))]
        public async Task<Despesa> Post(DespesaDto vendaDto)
            => await DespesaServices.InsertAsync(vendaDto);

        [HttpPut]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DespesaExample))]
        public async Task<Despesa> Put(int id, DespesaDto vendaDto)
            => await DespesaServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
        public async Task Delete(int id)
            => await DespesaServices.DeleteAsync(id);
    }
}