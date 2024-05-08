using Application.Interfaces.Utilities;
using CasaFinanceiroApi.APIValidators;
using Data.Configurations;
using Data.DataContext;
using DIContainer.DataBaseConfiguration.ConnectionString;
using Domain.Dtos.BaseResponse;
using Domain.Enumeradores;
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
        private readonly IConnectionStringResolver connectionStringResolver =
            service.GetRequiredService<IConnectionStringResolver>();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if(!_modelState.ValidarModelState(context))
                return;

            var connectionString = connectionStringResolver.IdentificarStringConexao(context);

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
    }
}
