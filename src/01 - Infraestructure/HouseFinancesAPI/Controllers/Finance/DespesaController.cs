using Domain.Dtos.Finance;
using Domain.Enumeradores;
using Domain.Models;
using HouseFinancesAPI.Attributes;
using HouseFinancesAPI.Utilities;
using Microsoft.AspNetCore.Mvc;
using HouseFinancesAPI.Controllers.Base;
using Application.Interfaces.Services.Finance;

namespace HouseFinancesAPI.Controllers.Finance
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
        public async Task<PagedResult<Despesa>> GetAllAsync(int paginaAtual = 1, int itensPorPagina = 10)
            => await DespesaServices.GetAllAsync(paginaAtual, itensPorPagina);

        [HttpGet("{id}")]
        public async Task<Despesa> GetById(int id)
            => await DespesaServices.GetByIdAsync(id);

        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Despesa> Post(DespesaDto vendaDto)
            => await DespesaServices.InsertAsync(vendaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Despesa> Put(int id, DespesaDto vendaDto)
            => await DespesaServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task Delete(int id)
            => await DespesaServices.DeleteAsync(id);
        #endregion


        [HttpPost("inserir-lote")]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<IEnumerable<Despesa>> PostRange(IAsyncEnumerable<DespesaDto> vendaDto)
           => await DespesaServices.InsertRangeAsync(vendaDto);

        [HttpGet("total-por-membro")]
        public async Task<RelatorioDespesasMensais> GetTotalParaCadaMembro() 
            => await DespesaServices.GetTotalParaCadaMembroAsync();

        [HttpGet("total-por-categoria")]
        public async Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoria()
            => await DespesaServices.GetTotalPorCategoriaAsync();

        [HttpGet("total-por-mes")]
        public async Task<PagedResult<DespesasPorMesDto>> GetTotaisComprasPorMesAsync(int paginaAtual = 1, int itensPorPagina = 10)
            => await DespesaServices.GetTotaisComprasPorMesAsync(paginaAtual, itensPorPagina);
    }
}