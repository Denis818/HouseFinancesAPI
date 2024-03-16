using Application.Interfaces.Services;
using Application.Utilities;
using Domain.Converters;
using Domain.Enumeradores;
using Domain.Interfaces.Repository;
using Domain.Models;
using ProEventos.API.Controllers.Base;
using System.Text.Json;

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
                await context.RequestServices.GetService<ILogApplicationServices>()
                                             .RegisterLog(EnumTypeLog.Exception, context, exception: ex);

                var message = $"Erro interno no servidor. {(_environmentHost.IsDevelopment() ? ex.Message : "")}";

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