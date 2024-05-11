using Application.Interfaces.Utilities;
using CasaFinanceiroApi.APIValidators;
using Data.Configurations;
using Data.DataContext;
using Domain.Dtos.BaseResponse;
using Domain.Enumeradores;
using Domain.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CasaFinanceiroApi.Base
{
    public abstract class BaseApiController(IServiceProvider service) : Controller
    {
        private readonly CompanyConnectionStrings _companyConnections =
            service.GetRequiredService<CompanyConnectionStrings>();

        private readonly INotifier _notifier = service.GetRequiredService<INotifier>();
        private readonly ModelStateValidator _modelState = new();

        private readonly FinanceDbContext _context = service.GetRequiredService<FinanceDbContext>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if(!_modelState.ValidarModelState(context))
                return;

            var connectionString = IdentificarStringConexao(context);

            if(string.IsNullOrEmpty(connectionString))
                return;

            _context.SetConnectionString(connectionString);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            if(context.Result is ObjectResult result)
            {
                context.Result = CustomResponse(result.Value);
            }
        }

        private IActionResult CustomResponse<TResponse>(TResponse content)
        {
            if(_notifier.HasNotifications(EnumTipoNotificacao.NotFount, out var notFount))
            {
                return NotFound(new ResponseDTO<TResponse>(content) { Mensagens = notFount });
            }

            if(_notifier.HasNotifications(EnumTipoNotificacao.ClientError, out var clientErrors))
            {
                return BadRequest(new ResponseDTO<TResponse>(content) { Mensagens = clientErrors });
            }

            if(_notifier.HasNotifications(EnumTipoNotificacao.ServerError, out var serverErrors))
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ResponseDTO<TResponse>(content) { Mensagens = serverErrors }
                );
            }

            _notifier.HasNotifications(EnumTipoNotificacao.Informacao, out var infoMessages);
            return Ok(new ResponseDTO<TResponse>(content) { Mensagens = infoMessages });
        }

        protected void Notificar(EnumTipoNotificacao tipo, string message) =>
            _notifier.Notify(tipo, message);

        private string IdentificarStringConexao(ActionExecutingContext context)
        {
            var originHeader = context.HttpContext.Request.Headers.Origin.FirstOrDefault();

            string domain = null;

            if(!string.IsNullOrEmpty(originHeader))
            {
                var originUri = new Uri(originHeader);
                domain = originUri.Host;
            }

            if(string.IsNullOrEmpty(domain))
            {
                domain = "localhost";
            }

            var empresaLocalizada = _companyConnections.List.FirstOrDefault(empresa =>
                empresa.NomeDominio == domain
            );

            if(empresaLocalizada == null)
            {
                context.Result = new BadRequestObjectResult(
                    new ResponseDTO<string>(
                        null,
                        [
                            new Notificacao(
                                $"O nome de domínio '{domain}' não existe",
                                EnumTipoNotificacao.ClientError
                            )
                        ]
                    )
                );
                return null;
            }

            return empresaLocalizada.ConnectionString;
        }
    }
}
