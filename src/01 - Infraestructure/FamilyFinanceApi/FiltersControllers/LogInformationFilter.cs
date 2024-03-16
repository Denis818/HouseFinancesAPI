using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services;
using Domain.Models;
using Domain.Enumeradores;

namespace Api.Vendas.FiltersControllers
{
    public class LogInformationFilter(ILogApplicationServices LogService) : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult objectResult)
            {
                await LogService.RegisterLog(EnumTypeLog.Information, context.HttpContext, objectResult);
            }

            await next();
        }   
    }
}
