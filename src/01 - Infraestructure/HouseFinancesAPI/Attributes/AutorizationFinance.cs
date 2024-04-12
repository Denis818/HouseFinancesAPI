using Application.Utilities;
using HouseFinancesAPI.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HouseFinancesAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AutorizationFinance : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                var response = new ResponseResultDTO<string>()
                {
                    Mensagens = [new Notificacao("Acesso não autorizado.")]
                };

                context.Result = new ObjectResult(response) { StatusCode = 401 };
                return;
            }
        }
    }
}
