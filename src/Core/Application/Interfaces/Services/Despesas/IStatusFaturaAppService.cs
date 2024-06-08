using Domain.Dtos.Despesas.Criacao;
using Domain.Enumeradores;
using Domain.Models.Despesas;

namespace Application.Services
{
    public interface IStatusFaturaAppService
    {
        Task<StatusFatura> UpdateAsync(EnumFaturaNome faturaNome, EnumStatusFatura status);
        Task<StatusFaturaDto> GetStatusFaturaDtoByNameAsync(string status);
    }
}
