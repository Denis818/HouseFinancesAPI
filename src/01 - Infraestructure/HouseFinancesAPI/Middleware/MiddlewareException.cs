using System.Text.Json;
using Application.Interfaces.Services.LogApp;
using Domain.Enumeradores;
using HouseFinancesAPI.Controllers.Base;

namespace ProEventos.API.Configuration.Middleware
{
    public class MiddlewareException(RequestDelegate next, IWebHostEnvironment environmentHost)
    {
        private readonly RequestDelegate _next = next;
        private readonly IWebHostEnvironment _environmentHost = environmentHost;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await context
                    .RequestServices.GetService<ILogAppServices>()
                    .RegisterLog(EnumTypeLog.Exception, context, exception: ex);

                var message =
                    $"Erro interno no servidor. {(_environmentHost.IsDevelopment() ? ex.Message : "")}";

                var response = new ResponseResultDTO<string>()
                {
                    Mensagens = [new(message, EnumTipoNotificacao.ServerError)]
                };

                context.Response.Headers.Append("content-type", "application/json; charset=utf-8");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
