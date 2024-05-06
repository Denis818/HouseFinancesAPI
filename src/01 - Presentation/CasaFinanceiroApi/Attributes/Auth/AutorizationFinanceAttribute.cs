using CasaFinanceiroApi.Base;
using Domain.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CasaFinanceiroApi.Attributes.Auth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AutorizationFinanceAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                var response = new ResponseDTO<string>()
                {
                    Mensagens = [new Notificacao("Você não esta autenticado.")]
                };

                context.Result = new ObjectResult(response) { StatusCode = 401 };
                return;
            }
        }
    }
}
