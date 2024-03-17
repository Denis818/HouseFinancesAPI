using Domain.Enumeradores;
using Domain.Interfaces.Repository.LogApp;
using Domain.Models.LogApp;
using FamilyFinanceApi.Attributes;
using FamilyFinanceApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Controllers.Base;

namespace FamilyFinanceApi.Controllers.LogApp
{
    [ApiController]
    [AutorizationFinance]
    [Route("api/[controller]")]
    [PermissoesFinance(EnumPermissoes.USU_000002)]
    public class LogApplicationController(IServiceProvider service,  ILogApplicationRepository LogRepository) : BaseApiController(service)
    {
        [HttpGet]
        public async Task<PagedResult<LogApplication>> GetAllLogs(int paginaAtual = 1, int itensPorPagina = 10)
           => await Pagination.PaginateResult(LogRepository.GetLogs(), paginaAtual, itensPorPagina);
    }
}
