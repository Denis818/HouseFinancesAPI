using Api.Vendas.Utilities;
using Domain.Models.Finance;

namespace Application.Interfaces.Services
{
    public interface IFinanceServices
    {
        Task<PagedResult<Despesa>> GetAllDespesaAsync(int paginaAtual, int itensPorPagina);
    }
}