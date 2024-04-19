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
        protected readonly IConfiguration _configuration =
            service.GetRequiredService<IConfiguration>();

        protected void Notificar(EnumTipoNotificacao tipo, string mesage) =>
            _notificador.Notificar(tipo, mesage);

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if(context.Result is ObjectResult result)
            {
                context.Result = CustomResponse(result.Value);
                return;
            }
            else if(context.Result is FileContentResult fileContent)
            {
                context.Result = CustomResponse(fileContent);
                return;
            }
            context.Result = CustomResponse<object>(null);
        }

        protected IActionResult CustomResponse<TResponse>(TResponse contentResponse)
        {
            if(_notificador.ListNotificacoes.Count > 0)
            {
                var listInformacoes = _notificador.ListNotificacoes.Where(item =>
                    item.StatusCode == EnumTipoNotificacao.Informacao
                );
                if(listInformacoes.Any())
                {
                    return CustomOkResult(
                        new ResponseResultDTO<TResponse>(contentResponse)
                        {
                            Mensagens = listInformacoes.ToArray()
                        }
                    );
                }

                var ListErros = _notificador.ListNotificacoes.Where(item =>
                    item.StatusCode == EnumTipoNotificacao.ClientError
                );
                if(ListErros.Any())
                {
                    return BadRequest(
                        new ResponseResultDTO<TResponse>(contentResponse)
                        {
                            Mensagens = ListErros.ToArray()
                        }
                    );
                }

                var listErrosInternos = _notificador.ListNotificacoes.Where(item =>
                    item.StatusCode == EnumTipoNotificacao.ServerError
                );
                if(listErrosInternos.Any())
                {
                    return new ObjectResult(
                        new ResponseResultDTO<TResponse>(contentResponse)
                        {
                            Mensagens = listErrosInternos.ToArray()
                        }
                    )
                    {
                        StatusCode = 500
                    };
                }
            }

            return CustomOkResult(
                new ResponseResultDTO<TResponse>(contentResponse)
                {
                    Mensagens = [new Notificacao("")]
                }
            );
        }

        private IActionResult CustomOkResult(object result)
        {
            if(result is ResponseResultDTO<FileContentResult> fileDto)
            {
                return File(fileDto.Dados.FileContents, "application/pdf", "arquivo.pdf");
            }

            return Ok(result);
        }
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

        public ResponseResultDTO() { }
    }
}
