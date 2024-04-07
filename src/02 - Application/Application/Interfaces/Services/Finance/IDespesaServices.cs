using Domain.Dtos.Finance;
using Domain.Models.Finance;
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
        Task<ResumoMensalDto> GetResumoDespesasMensalAsync();
        Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoriaAsync();
        Task<IEnumerable<DespesasPorMesDto>> GetTotaisComprasPorMesAsync();
    }
}