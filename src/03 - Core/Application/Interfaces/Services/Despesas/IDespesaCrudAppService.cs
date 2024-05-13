using Application.Utilities;
using Domain.Dtos.Despesas.Criacao;
using Domain.Models.Despesas;

namespace Application.Interfaces.Services.Despesas
{
    public interface IDespesaCrudAppService
    {
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<Despesa>> GetAllAsync(int paginaAtual, int itensPorPagina);
        Task<PagedResult<Despesa>> FiltrarDespesaPorItem(
            string filterItem,
            int paginaAtual,
            int itensPorPagina
        );
        Task<Despesa> GetByIdAsync(int id);
        Task<Despesa> InsertAsync(DespesaDto despesaDto);
        Task<Despesa> UpdateAsync(int id, DespesaDto despesaDto);
        Task<IEnumerable<Despesa>> InsertRangeAsync(IAsyncEnumerable<DespesaDto> listDespesasDto);
    }
}
