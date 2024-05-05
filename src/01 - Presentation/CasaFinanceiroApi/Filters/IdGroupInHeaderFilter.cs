using Application.Interfaces.Utilities;
using Application.Resources.Messages;
using Domain.Enumeradores;
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
            else
            {
                var notifier = context.HttpContext.RequestServices.GetService<INotifier>();

                notifier.Notify(EnumTipoNotificacao.ClientError, Message.GrupoDespesaNaoEncontrado);
                context.Result = null;
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context) { }
    }
}
