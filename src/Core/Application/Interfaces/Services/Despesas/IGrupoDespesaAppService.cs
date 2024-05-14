using Domain.Dtos.Despesas.Criacao;
using Domain.Models.Despesas;

namespace Application.Interfaces.Services.Despesas
{
    public interface IGrupoDespesaAppService
    {
        Task<IEnumerable<GrupoDespesa>> GetAllAsync();
        Task<GrupoDespesa> InsertAsync(GrupoDespesaDto grupoDto);
        Task<GrupoDespesa> UpdateAsync(int id, GrupoDespesaDto grupoDespesaDto);
        Task<bool> DeleteAsync(int id);
    }
}