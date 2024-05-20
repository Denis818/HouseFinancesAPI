using Application.Utilities;
using Domain.Dtos.Despesas.Criacao;
using Domain.Enumeradores;
using Domain.Models.Despesas;

namespace Application.Interfaces.Services.Despesas
{
    public interface IDespesaCrudAppService
    {
        Task<PagedResult<Despesa>> GetListDespesasPorGrupo(
            string filter,
            int paginaAtual,
            int itensPorPagina,
            EnumFiltroDespesa tipoFiltro
        );

        Task<PagedResult<Despesa>> GetListDespesasAllGroups(
            string filter,
            int paginaAtual,
            int itensPorPagina,
            EnumFiltroDespesa tipoFiltro
        );

        Task<bool> DeleteAsync(int id);
        Task<Despesa> GetByIdAsync(int id);
        Task<Despesa> InsertAsync(DespesaDto despesaDto);
        Task<Despesa> UpdateAsync(int id, DespesaDto despesaDto);
        Task<IEnumerable<Despesa>> InsertRangeAsync(IAsyncEnumerable<DespesaDto> listDespesasDto);
    }
}
