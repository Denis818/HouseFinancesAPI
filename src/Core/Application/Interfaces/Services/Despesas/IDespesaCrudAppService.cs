using Application.Utilities;
using Domain.Dtos.Despesas.Criacao;
using Domain.Enumeradores;
using Domain.Models.Despesas;

namespace Application.Interfaces.Services.Despesas
{
    public interface IDespesaCrudAppService
    {
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<Despesa>> GetListDespesas(
            string filterItem,
            EnumFiltroDespesa tipoFiltro,
            int paginaAtual,
            int itensPorPagina
        );
        Task<Despesa> GetByIdAsync(int id);
        Task<Despesa> InsertAsync(DespesaDto despesaDto);
        Task<Despesa> UpdateAsync(int id, DespesaDto despesaDto);
        Task<IEnumerable<Despesa>> InsertRangeAsync(IAsyncEnumerable<DespesaDto> listDespesasDto);
    }
}
