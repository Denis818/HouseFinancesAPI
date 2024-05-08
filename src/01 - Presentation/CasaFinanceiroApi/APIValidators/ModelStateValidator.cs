using Application.Resources.Messages;
using Domain.Dtos.BaseResponse;
using Domain.Enumeradores;
using Domain.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CasaFinanceiroApi.APIValidators
{
    public class ModelStateValidator
    {
        public bool ValidarModelState(ActionExecutingContext context)
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
}
