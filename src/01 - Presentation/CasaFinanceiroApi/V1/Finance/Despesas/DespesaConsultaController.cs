using Application.Interfaces.Services.Despesas;
using Asp.Versioning;
using CasaFinanceiroApi.Attributes.Auth;
using CasaFinanceiroApi.Base;
using Domain.Dtos.Despesas.Consultas;
using Microsoft.AspNetCore.Mvc;

namespace CasaFinanceiroApi.V1.Finance.Despesas
{
    [ApiController]
    [ApiVersion("1")]
    [AutorizationFinance]
    [Route("api/v1/despesa")]
    public class DespesaConsultaController(
        IServiceProvider service,
        IDespesaConsultaAppService _despesaConsultaApp
    ) : BaseApiController(service)
    {
        #region Consultas
        [HttpGet("resumo-despesas-mensal")]
        public async Task<DespesasDivididasMensalDto> GetResumoDespesasMensalAsync() =>
            await _despesaConsultaApp.GetDespesasDivididasMensalAsync();

        [HttpGet("total-por-categoria")]
        public async Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoriaAsync() =>
            await _despesaConsultaApp.GetTotalPorCategoriaAsync();

        [HttpGet("total-por-mes")]
        public async Task<IEnumerable<DespesasPorMesDto>> GetTotaisComprasPorGrupoParaGraficoAsync() =>
            await _despesaConsultaApp.GetTotaisComprasPorGrupoParaGraficoAsync();
        #endregion
    }
}
