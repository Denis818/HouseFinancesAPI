using Application.Interfaces.Services;
using Domain.Dtos.Finance;
using Domain.Models;
using FamilyFinanceApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Controllers.Base;

namespace FamilyFinanceApi.Controllers.Finance
{
    [ApiController]
    // [AutorizationFinance]
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
        //  [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Despesa> Post(DespesaDto vendaDto)
            => await DespesaServices.InsertAsync(vendaDto);

        [HttpPut]
        //  [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Despesa> Put(int id, DespesaDto vendaDto)
            => await DespesaServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
        // [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task Delete(int id)
            => await DespesaServices.DeleteAsync(id);
        #endregion

        [HttpGet("total-por-membro")]
        public async Task<DespesasPorMembroDto> GetTotalParaCadaMembro() 
            => await DespesaServices.GetTotalParaCadaMembro();

        [HttpGet("total-por-categoria")]
        public async Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoria()
            => await DespesaServices.GetTotalPorCategoria();

        [HttpGet("totais-por-mes")]
        public async Task<PagedResult<DespesasPorMesDto>> GetTotalDespesasByMonthAsync(int paginaAtual = 1, int itensPorPagina = 10)
            => await DespesaServices.GetTotaisComprasPorMesAsync(paginaAtual, itensPorPagina);
    }
}