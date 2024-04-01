using Domain.Enumeradores;
using Domain.Interfaces;
using Domain.Models;
using HouseFinancesAPI.Attributes;
using HouseFinancesAPI.Utilities;
using Microsoft.AspNetCore.Mvc;
using HouseFinancesAPI.Controllers.Base;

namespace HouseFinancesAPI.Controllers.LogApp
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
