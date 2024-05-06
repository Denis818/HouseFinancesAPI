using Application.Resources.Messages;
using CasaFinanceiroApi.Base;
using Domain.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CasaFinanceiroApi.Filters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IdGroupInHeaderFilterAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var httpContext = context.HttpContext;
            string grupoId = httpContext.Request.Headers["Grupo-Despesas-Id"];

            if (int.TryParse(grupoId, out int grupoDespesasId))
            {
                httpContext.Items["GrupoDespesaId"] = grupoDespesasId;
            }
            else if (httpContext.Request.Method == "GET")
            {
                context.Result = new BadRequestObjectResult(
                    new ResponseDTO<string>()
                    {
                        Mensagens =
                        [
                            new(Message.SelecioneUmGrupoDesesa, EnumTipoNotificacao.ClientError)
                        ]
                    }
                );
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context) { }
    }
}
