using Application.Interfaces.Utilities;
using Application.Utilities;
using Domain.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HouseFinancesAPI.Controllers.Base
{
    public abstract class BaseApiController(IServiceProvider service) : Controller
    {
        private readonly INotifier _notificador = service.GetRequiredService<INotifier>();
        protected readonly IConfiguration _configuration = service.GetRequiredService<IConfiguration>();

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if(context.Result is not ObjectResult result)
            {
                context.Result = CustomResponse<object>(null);
                return;
            }
            context.Result = CustomResponse(result.Value);
        }

        protected IActionResult CustomResponse<TResponse>(TResponse contentResponse)
        {
            if(_notificador.ListNotificacoes.Count > 0)
            {
                var listInformacoes = _notificador.ListNotificacoes.Where(item => item.StatusCode == EnumTipoNotificacao.Informacao);
                if(listInformacoes.Any())
                {
                    return Ok(new ResponseResultDTO<TResponse>(contentResponse)
                    {
                        Mensagens = listInformacoes.ToArray()
                    });
                }

                var ListErros = _notificador.ListNotificacoes.Where(item => item.StatusCode == EnumTipoNotificacao.ClientError);
                if(ListErros.Any())
                {
                    return BadRequest(new ResponseResultDTO<TResponse>(contentResponse)
                    {
                        Mensagens = ListErros.ToArray()
                    });
                }

                var listErrosInternos = _notificador.ListNotificacoes.Where(item => item.StatusCode == EnumTipoNotificacao.ServerError);
                if(listErrosInternos.Any())
                {
                    return new ObjectResult(new ResponseResultDTO<TResponse>(contentResponse)
                    {
                        Mensagens = listErrosInternos.ToArray()

                    })
                    { StatusCode = 500 };
                }
            }

            return Ok(new ResponseResultDTO<TResponse>(contentResponse)
            {
                Mensagens = [new Notificacao("")]
            });
        }

        protected void Notificar(EnumTipoNotificacao tipo, string mesage)
            => _notificador.Notificar(tipo, mesage);
    }

    public class ResponseResultDTO<TResponse>
    {
        public TResponse Dados { get; set; }
        public Notificacao[] Mensagens { get; set; }

        public ResponseResultDTO(TResponse data, Notificacao[] notificacoes = null)
        {
            Dados = data;
            Mensagens = notificacoes;
        }
        public ResponseResultDTO()
        {
        }
    }
}
