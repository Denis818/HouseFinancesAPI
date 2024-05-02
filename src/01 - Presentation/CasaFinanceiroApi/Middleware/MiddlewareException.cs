using CasaFinanceiroApi.Base;
using Domain.Enumeradores;
using System.Text.Json;

namespace CasaFinanceiroApi.Middleware
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
            catch(Exception ex)
            {
                var message =
                    $"Erro interno no servidor. {(_environmentHost.IsDevelopment() ? ex.Message : "")}";

                var response = new ResponseDTO<string>()
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
