using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CasaFinanceiroApi.Extensios.Swagger.Filters
{
    public class GrupoDespesaIdHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var controllerName = context.MethodInfo.DeclaringType.Name;

            if(controllerName.StartsWith("Despesa") && context.ApiDescription.HttpMethod == "GET")
            {
                operation.Parameters ??= [];

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Grupo-Despesas-Id",
                    In = ParameterLocation.Header,
                    Description = "Adiciona um Id de um grupo de despesas no cabeçario da requisição",
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "int"
                    }
                });
            }
        }
    }

}
