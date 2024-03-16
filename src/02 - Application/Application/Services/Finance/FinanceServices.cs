using Api.Vendas.Utilities;
using Application.Interfaces.Services;
using Application.Services.Base;
using Domain.Interfaces.Repository.Finance;
using Domain.Models.Dtos;
using Domain.Models.Finance;

namespace Application.Services.Finance
{
    public class FinanceServices(IServiceProvider service) :
        ServiceAppBase<Despesa, DespesaDto, IFinanceRepository>(service), IFinanceServices
    {
        public async Task<PagedResult<Despesa>> GetAllDespesaAsync(int paginaAtual, int itensPorPagina)
            => await Pagination.PaginateResult(_repository.Get(), paginaAtual, itensPorPagina);
    }
}
