using Domain.Dtos.Despesas.Criacao;
using Domain.Models.Despesas;

namespace Application.Interfaces.Services.Despesas
{
    public interface IDespesaCrudAppService
    {
        Task<bool> DeleteAsync(int id);
        Task<Despesa> InsertAsync(DespesaDto despesaDto);
        Task<Despesa> UpdateAsync(int id, DespesaDto despesaDto);
        Task<IEnumerable<Despesa>> InsertRangeAsync(IAsyncEnumerable<DespesaDto> listDespesasDto);
    }
}
