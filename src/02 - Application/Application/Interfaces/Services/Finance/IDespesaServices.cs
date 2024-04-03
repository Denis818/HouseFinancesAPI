using Domain.Dtos.Finance;
using Domain.Models;
using HouseFinancesAPI.Utilities;

namespace Application.Interfaces.Services.Finance
{
    public interface IDespesaServices
    {
        Task DeleteAsync(int id);
        Task<PagedResult<Despesa>> GetAllAsync(int paginaAtual, int itensPorPagina);
        Task<Despesa> GetByIdAsync(int id);
        Task<Despesa> InsertAsync(DespesaDto despesaDto);
        Task<Despesa> UpdateAsync(int id, DespesaDto despesaDto);

        Task<IEnumerable<Despesa>> InsertRangeAsync(IAsyncEnumerable<DespesaDto> listDespesasDto);
        Task<RelatorioDespesasMensais> GetTotalParaCadaMembroAsync();
        Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoriaAsync();
        Task<IEnumerable<DespesasPorMesDto>> GetTotaisComprasPorMesAsync();
    }
}