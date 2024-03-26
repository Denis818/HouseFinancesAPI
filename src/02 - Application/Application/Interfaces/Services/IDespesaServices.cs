using Domain.Dtos.Finance;
using Domain.Dtos.Responses;
using Domain.Models;
using FamilyFinanceApi.Utilities;

namespace Application.Interfaces.Services
{
    public interface IDespesaServices
    {
        Task DeleteAsync(int id);
        Task<PagedResult<Despesa>> GetAllAsync(int paginaAtual, int itensPorPagina);
        Task<Despesa> GetByIdAsync(int id);
        Task<Despesa> InsertAsync(DespesaDto despesaDto);
        Task<Despesa> UpdateAsync(int id, DespesaDto despesaDto);
        Task<DespesasMensaisPorMembroDto> GetTotalParaCadaMembro();
        Task<PagedResult<DespesasPorMesDto>> GetTotaisComprasPorMesAsync(int paginaAtual, int itensPorPagina);
    }
}