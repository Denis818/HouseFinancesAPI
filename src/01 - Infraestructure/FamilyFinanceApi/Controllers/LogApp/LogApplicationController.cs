using Domain.Enumeradores;
using Domain.Interfaces;
using Domain.Models;
using FamilyFinanceApi.Attributes;
using FamilyFinanceApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using FamilyFinanceApi.Controllers.Base;

namespace FamilyFinanceApi.Controllers.LogApp
{
    [ApiController]
    [AutorizationFinance]
    [Route("api/[controller]")]
    [PermissoesFinance(EnumPermissoes.USU_000002)]
    public class LogApplicationController(IServiceProvider service,  ILogApplicationRepository LogRepository) : BaseApiController(service)
    {
        [HttpGet]
        public async Task<PagedResult<LogApplication>> GetAllAsync(int paginaAtual = 1, int itensPorPagina = 10)
           => await Pagination.PaginateResultAsync(LogRepository.GetLogs(), paginaAtual, itensPorPagina);
    }
}
