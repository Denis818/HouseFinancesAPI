using CasaFinanceiroApi.Filters;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CasaFinanceiroApi.Extensios.Swagger.Filters
{
    public class GrupoDespesaIdHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var controllerPrecisaIdGrupo = context
                .MethodInfo.DeclaringType.GetCustomAttributes(true)
                .OfType<GetIdGroupInHeaderFilterAttribute>()
                .Any();

            var endPointPrecisaIdGrupo = context
                .MethodInfo.GetCustomAttributes(true)
                .OfType<GetIdGroupInHeaderFilterAttribute>()
                .Any();

            if(
                (controllerPrecisaIdGrupo || endPointPrecisaIdGrupo)
                && context.ApiDescription.HttpMethod == "GET"
            )
            {
                operation.Parameters ??= new List<OpenApiParameter>();

                operation.Parameters.Add(
                    new OpenApiParameter
                    {
                        Name = "Grupo-Despesas-Id",
                        In = ParameterLocation.Header,
                        Description =
                            "Adiciona um Id de um grupo de despesas no cabeçalho da requisição",
                        Required = false,
                        Schema = new OpenApiSchema { Type = "int" }
                    }
                );
            }
        }
    }
}
