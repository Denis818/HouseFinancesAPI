using Domain.Dtos.Finance;
using Domain.Models.Finance;
using FamilyFinanceApi.Utilities;

namespace Application.Interfaces.Services
{
    public interface IDespesaServices
    {
        Task DeleteAsync(int id);
        Task<PagedResult<Despesa>> GetAllDespesaAsync(int paginaAtual, int itensPorPagina);
        Task<Despesa> GetByIdAsync(int id);
        Task<Despesa> InsertAsync(DespesaDto despesaDto);
        Task<Despesa> UpdateAsync(int id, DespesaDto despesaDto);
    }
}