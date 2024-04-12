using Application.Interfaces.Services.Finance;
using Domain.Dtos.Finance;
using Domain.Enumeradores;
using Domain.Models.Finance;
using HouseFinancesAPI.Attributes;
using HouseFinancesAPI.Controllers.Base;
using HouseFinancesAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace HouseFinancesAPI.Controllers.Finance
{
    [ApiController]
    [AutorizationFinance]
    [Route("api/[controller]")]
    public class DespesaController(IServiceProvider service, IDespesaAppServices _despesaServices)
        : BaseApiController(service)
    {
        #region CRUD
        [HttpGet]
        public async Task<PagedResult<Despesa>> GetAllAsync(
            int paginaAtual = 1,
            int itensPorPagina = 10
        ) => await _despesaServices.GetAllAsync(paginaAtual, itensPorPagina);

        [HttpGet("{id}")]
        public async Task<Despesa> GetById(int id) => await _despesaServices.GetByIdAsync(id);

        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Despesa> Post(DespesaDto vendaDto) =>
            await _despesaServices.InsertAsync(vendaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Despesa> Put(int id, DespesaDto vendaDto) =>
            await _despesaServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task Delete(int id) => await _despesaServices.DeleteAsync(id);
        #endregion


        [HttpPost("inserir-lote")]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<IEnumerable<Despesa>> PostRange(IAsyncEnumerable<DespesaDto> vendaDto) =>
            await _despesaServices.InsertRangeAsync(vendaDto);

        [HttpGet("resumo-despesas-mensal")]
        public async Task<ResumoMensalDto> GetResumoDespesasMensalAsync() =>
            await _despesaServices.GetResumoDespesasMensalAsync();

        [HttpGet("total-por-categoria")]
        public async Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoria() =>
            await _despesaServices.GetTotalPorCategoriaAsync();

        [HttpGet("total-por-mes")]
        public async Task<IEnumerable<DespesasPorMesDto>> GetTotaisComprasPorMesAsync() =>
            await _despesaServices.GetTotaisComprasPorMesAsync();
    }
}
