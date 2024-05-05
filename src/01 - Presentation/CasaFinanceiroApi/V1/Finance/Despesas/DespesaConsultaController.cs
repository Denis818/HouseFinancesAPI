using Application.Interfaces.Services.Despesas;
using Asp.Versioning;
using CasaFinanceiroApi.Attributes.Auth;
using CasaFinanceiroApi.Base;
using CasaFinanceiroApi.Filters;
using Domain.Dtos.Despesas.Consultas;
using Microsoft.AspNetCore.Mvc;

namespace CasaFinanceiroApi.V1.Finance.Despesas
{
    [ApiController]
    [ApiVersion("1")]
    [AutorizationFinance]
    [IdGroupInHeaderFilter]
    [Route("api/v1/despesa")]
    public class DespesaConsultaController(
        IServiceProvider service,
        IDespesaConsultaAppService _despesaConsultaApp
    ) : BaseApiController(service)
    {
        #region Consultas
        [HttpGet("analise-despesa-por-grupo")]
        public async Task<DespesasDivididasMensalDto> GetAnaliseDesesasPorGrupoAsync() =>
            await _despesaConsultaApp.GetAnaliseDesesasPorGrupoAsync();

        [HttpGet("total-por-categoria")]
        public async Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoriaAsync() =>
            await _despesaConsultaApp.GetTotalPorCategoriaAsync();

        [HttpGet("total-por-grupo")]
        public async Task<IEnumerable<DespesasPorMesDto>> GetDespesaGrupoParaGraficoAsync() =>
            await _despesaConsultaApp.GetDespesaGrupoParaGraficoAsync();
        #endregion
    }
}
