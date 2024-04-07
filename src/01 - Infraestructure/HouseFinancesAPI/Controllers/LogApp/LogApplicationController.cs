using Domain.Enumeradores;
using Domain.Interfaces;
using HouseFinancesAPI.Attributes;
using HouseFinancesAPI.Utilities;
using Microsoft.AspNetCore.Mvc;
using HouseFinancesAPI.Controllers.Base;
using Domain.Models.LogApp;

namespace HouseFinancesAPI.Controllers.LogApp
{
    [ApiController]
    [AutorizationFinance]
    [Route("api/[controller]")]
    [PermissoesFinance(EnumPermissoes.USU_000002)]
    public class LogApplicationController(IServiceProvider service,  ILogApplicationRepository LogRepository) : BaseApiController(service)
    {
        [HttpGet]
        public async Task<PagedResult<LogRequest>> GetAllAsync(int paginaAtual = 1, int itensPorPagina = 10)
           => await Pagination.PaginateResultAsync(LogRepository.GetLogs(), paginaAtual, itensPorPagina);
    }
}
