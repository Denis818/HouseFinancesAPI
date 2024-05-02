using Application.Interfaces.Utilities;
using Application.Resources.Messages;
using Domain.Enumeradores;
using Domain.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CasaFinanceiroApi.Controllers.Base
{
    public abstract class BaseApiController(IServiceProvider service) : Controller
    {
        private readonly INotifier _notifier = service.GetRequiredService<INotifier>();

        protected readonly IConfiguration _configuration =
            service.GetRequiredService<IConfiguration>();

        protected void Notificar(EnumTipoNotificacao tipo, string message) =>
            _notifier.Notify(tipo, message);

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if(!ValidarModelState(context))
                return;
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

        private bool ValidarModelState(ActionExecutingContext context)
        {
            var modelState = context.ModelState;
            if(!modelState.IsValid)
            {
                if(!ValidarContentTypeRequest(modelState, context))
                    return false;

                var valoresInvalidosModelState = modelState.Where(x =>
                    x.Value.ValidationState == ModelValidationState.Invalid
                );
                if(!valoresInvalidosModelState.Any())
                    return true;

                ExtrairMensagensDeErroDaModelState(valoresInvalidosModelState, context);
                return false;
            }
            return true;
        }

        private void ExtrairMensagensDeErroDaModelState(
        IEnumerable<KeyValuePair<string, ModelStateEntry>> valoresInvalidosModelState,
        ActionExecutingContext context
    )
        {
            var listaErros = new List<Notificacao>();
            foreach(var model in valoresInvalidosModelState)
            {
                var nomeCampo = model.Key.StartsWith("$.") ? model.Key.Substring(2) : model.Key;
                listaErros.Add(
                    new Notificacao(
                        string.Format(Message.CampoFormatoInvalido, nomeCampo),
                        EnumTipoNotificacao.ClientError
                    )
                );
            }

            context.Result = new BadRequestObjectResult(
                new ResponseDTO<string>(null, listaErros.ToArray())
            );
        }

        private bool ValidarContentTypeRequest(
            ModelStateDictionary modelState,
            ActionExecutingContext context
        )
        {
            var valoresInvalidosModelState = modelState.Where(x =>
                x.Value.ValidationState == ModelValidationState.Invalid
            );
            if(valoresInvalidosModelState.Count() == 1)
            {
                var model = valoresInvalidosModelState.First();
                var modelErro = model.Value.Errors.FirstOrDefault();

                if(model.Key == string.Empty && modelErro.ErrorMessage == string.Empty)
                {
                    var result = new ResponseDTO<object>();
                    result.ContentTypeInvalido();
                    context.Result = new BadRequestObjectResult(result);
                    return false;
                }
                else if(model.Key == string.Empty)
                {
                    model.Value.ValidationState = ModelValidationState.Valid;
                    return true;
                }
            }
            return true;
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

        public void ContentTypeInvalido()
        {
            Mensagens =
            [
                new("Content-Type inválido.", EnumTipoNotificacao.ClientError)
            ];
        }

    }
}
