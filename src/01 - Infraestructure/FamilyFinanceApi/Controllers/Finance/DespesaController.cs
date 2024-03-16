using Api.Vendas.Attributes;
using Api.Vendas.Utilities;
using Application.Interfaces.Services;
using Domain.Models.Finance;
using FamilyFinanceApi.Attributes;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Controllers.Base;

namespace FamilyFinanceApi.Controllers.Finance
{
    [ApiController]
    [PermissoesFinance]
    [AutorizationFinance]
    [Route("api/[controller]")]
    public class DespesaController(IServiceProvider service, 
        IFinanceServices FinanceServices) : 
        BaseApiController(service)
    {
        [HttpGet]
        public async Task<PagedResult<Despesa>> GetAllDespesaAsync(int paginaAtual = 1, int itensPorPagina = 10) =>
            await FinanceServices.GetAllDespesaAsync(paginaAtual, itensPorPagina);
        
    }
}
