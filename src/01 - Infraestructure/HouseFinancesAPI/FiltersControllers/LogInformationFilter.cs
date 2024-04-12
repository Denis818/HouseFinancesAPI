using Application.Interfaces.Services.LogApp;
using Domain.Enumeradores;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HouseFinancesAPI.FiltersControllers
{
    public class LogInformationFilter(ILogAppServices LogService) : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(
            ResultExecutingContext context,
            ResultExecutionDelegate next
        )
        {
            if (context.Result is ObjectResult objectResult)
            {
                await LogService.RegisterLog(
                    EnumTypeLog.Information,
                    context.HttpContext,
                    objectResult
                );
            }

            await next();
        }
    }
}
