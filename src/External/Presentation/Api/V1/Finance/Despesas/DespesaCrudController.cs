using Application.Interfaces.Services.Despesas;
using Application.Utilities;
using Asp.Versioning;
using Domain.Dtos.Despesas.Criacao;
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
    [Route("api/v1/despesa")]
    public class DespesaCrudController(
        IServiceProvider service,
        IDespesaCrudAppService _despesaCrudServices
    ) : MainController(service)
    {
        #region CRUD
        [HttpGet("por-grupo")]
        [GetIdGroupInHeaderFilter]
        public async Task<PagedResult<Despesa>> GetListDespesasPorGrupo(
            string filter,
            int paginaAtual = 1,
            int itensPorPagina = 10,
            EnumFiltroDespesa tipoFiltro = EnumFiltroDespesa.Item
        )
        {
            return await _despesaCrudServices.GetListDespesasPorGrupo(
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
            return await _despesaCrudServices.GetListDespesasAllGroups(
                filter,
                paginaAtual,
                itensPorPagina,
                tipoFiltro
            );
        }

        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Despesa> PostAsync(DespesaDto vendaDto) =>
            await _despesaCrudServices.InsertAsync(vendaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<Despesa> PutAsync(int id, DespesaDto vendaDto) =>
            await _despesaCrudServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task<bool> DeleteAsync(int id) => await _despesaCrudServices.DeleteAsync(id);

        [HttpPost("inserir-lote")]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<IEnumerable<Despesa>> PostRangeAsync(
            IAsyncEnumerable<DespesaDto> vendaDto
        ) => await _despesaCrudServices.InsertRangeAsync(vendaDto);
        #endregion
    }
}
