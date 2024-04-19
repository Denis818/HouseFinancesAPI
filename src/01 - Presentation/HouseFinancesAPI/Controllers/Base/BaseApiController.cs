using Application.Interfaces.Utilities;
using Domain.Enumeradores;
using Domain.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HouseFinancesAPI.Controllers.Base
{
    public abstract class BaseApiController(IServiceProvider service) : Controller
    {
        protected readonly INotifier _notifier = service.GetRequiredService<INotifier>();

        protected readonly IConfiguration _configuration =
            service.GetRequiredService<IConfiguration>();

        protected void Notificar(EnumTipoNotificacao tipo, string message) =>
            _notifier.Notify(tipo, message);

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
            if(_notifier.HasNotifications(EnumTipoNotificacao.ClientError, out var clientErrors))
            {
                return BadRequest(new ResponseDTO<TResponse>(content) { Mensagens = clientErrors });
            }

            if(_notifier.HasNotifications(EnumTipoNotificacao.ServerError, out var serverErrors))
            {
                return StatusCode(
                    500,
                    new ResponseDTO<TResponse>(content) { Mensagens = serverErrors }
                );
            }

            _notifier.HasNotifications(EnumTipoNotificacao.Informacao, out var infoMessages);
            return Ok(new ResponseDTO<TResponse>(content) { Mensagens = infoMessages });
        }
    }

    public class ResponseDTO<T>
    {
        public T Dados { get; set; }
        public Notificacao[] Mensagens { get; set; }

        public ResponseDTO(T data, Notificacao[] messages = null)
        {
            Dados = data;
            Mensagens = messages ?? [];
        }

        public ResponseDTO() { }
    }
}
