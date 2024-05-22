using Application.Interfaces.Services.Despesas;
using Application.Utilities;
using Asp.Versioning;
using Domain.Dtos.Despesas.Consultas;
using Domain.Enumeradores;
using Domain.Models.Despesas;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Base;
using Presentation.Attributes.Auth;
using Presentation.Attributes.Util;
using Presentation.Version;

namespace Presentation.Api.V1.Finance.Despesas
{
    [ApiController]
    [ApiVersion(ApiVersioning.V1)]
    [AutorizationFinance]
    [GetIdGroupInHeaderFilter]
    [Route("api/v1/despesa")]
    public class DespesaConsultaController(
        IServiceProvider service,
        IDespesaConsultas _despesaConsultas
    ) : MainController(service)
    {
        #region Listagem das Despesas
        [HttpGet("por-grupo")]
        [GetIdGroupInHeaderFilter]
        public async Task<PagedResult<Despesa>> GetListDespesasPorGrupo(
            string filter,
            int paginaAtual = 1,
            int itensPorPagina = 10,
            EnumFiltroDespesa tipoFiltro = EnumFiltroDespesa.Item
        )
        {
            return await _despesaConsultas.GetListDespesasPorGrupo(
                filter,
                paginaAtual,
                itensPorPagina,
                tipoFiltro
            );
        }

        [HttpGet("todos-grupos")]
        public async Task<PagedResult<Despesa>> GetListDespesasAllGrupos(
            string filter,
            int paginaAtual = 1,
            int itensPorPagina = 10,
            EnumFiltroDespesa tipoFiltro = EnumFiltroDespesa.Item
        )
        {
            return await _despesaConsultas.GetListDespesasAllGroups(
                filter,
                paginaAtual,
                itensPorPagina,
                tipoFiltro
            );
        }
        #endregion

        #region Análise das Despesas
        [HttpGet("analise-despesa-por-grupo")]
        public async Task<DespesasDivididasMensalDto> GetAnaliseDesesasPorGrupoAsync() =>
            await _despesaConsultas.GetAnaliseDesesasPorGrupoAsync();

        [HttpGet("total-por-categoria")]
        public async Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoriaAsync() =>
            await _despesaConsultas.GetTotalPorCategoriaAsync();

        [HttpGet("total-por-grupo")]
        public async Task<IEnumerable<DespesasPorGrupoDto>> GetDespesaGrupoParaGraficoAsync() =>
            await _despesaConsultas.GetDespesaGrupoParaGraficoAsync();
        #endregion
    }
}
