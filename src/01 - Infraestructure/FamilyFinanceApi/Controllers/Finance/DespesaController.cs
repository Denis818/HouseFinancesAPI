using Application.Interfaces.Services;
using Domain.Dtos.Finance;
using Domain.Dtos.Responses;
using Domain.Enumeradores;
using Domain.Models;
using FamilyFinanceApi.Attributes;
using FamilyFinanceApi.Extensios.Swagger.ExamplesSwagger;
using FamilyFinanceApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Controllers.Base;
using Swashbuckle.AspNetCore.Filters;

namespace FamilyFinanceApi.Controllers.Finance
{
    [ApiController]
    [AutorizationFinance]
    [Route("api/[controller]")]
    public class DespesaController(IServiceProvider service, 
        IDespesaServices DespesaServices) : 
        BaseApiController(service)
    {
        #region CRUD
        [HttpGet]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PageDespesaExample))]
        public async Task<PagedResult<Despesa>> GetAllDespesaAsync(int paginaAtual = 1, int itensPorPagina = 10) =>
            await DespesaServices.GetAllDespesaAsync(paginaAtual, itensPorPagina);

        [HttpGet("{id}")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DespesaExample))]
        public async Task<Despesa> GetById(int id) => await DespesaServices.GetByIdAsync(id);

        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DespesaExample))]
        public async Task<Despesa> Post(DespesaDto vendaDto) => await DespesaServices.InsertAsync(vendaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DespesaExample))]
        public async Task<Despesa> Put(int id, DespesaDto vendaDto) => await DespesaServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task Delete(int id) => await DespesaServices.DeleteAsync(id);
        #endregion

        [HttpGet("total-por-membro")]
        public async Task<DespesasMensaisPorMembroDto> GetTotalParaCadaMembro() =>
            await DespesaServices.GetTotalParaCadaMembro();

        [HttpGet("totais-por-mes")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PageDespesabyMonthExample))]
        public async Task<PagedResult<DespesasPorMesDto>> GetTotalDespesasByMonthAsync(int paginaAtual = 1, int itensPorPagina = 10) =>
           await DespesaServices.GetTotaisComprasPorMesAsync(paginaAtual, itensPorPagina);
    }
}