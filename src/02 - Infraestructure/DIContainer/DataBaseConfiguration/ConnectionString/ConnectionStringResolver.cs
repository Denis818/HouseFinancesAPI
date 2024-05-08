using Data.Configurations;
using Domain.Dtos.BaseResponse;
using Domain.Enumeradores;
using Domain.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DIContainer.DataBaseConfiguration.ConnectionString
{
    public class ConnectionStringResolver(
        IServiceProvider service,
        IHttpContextAccessor _httpContext
    ) : IConnectionStringResolver
    {
        private readonly CompanyConnectionStrings _companyConnections =
            service.GetRequiredService<CompanyConnectionStrings>();

        public string IdentificarStringConexao()
        {
            var originHeader = _httpContext.HttpContext.Request.Headers["Origin"].FirstOrDefault();

            string domain = null;

            if(!string.IsNullOrEmpty(originHeader))
            {
                var originUri = new Uri(originHeader);
                domain = originUri.Host;
            }

            if(string.IsNullOrEmpty(domain))
            {
                throw new InvalidOperationException(
                    "O cabeçalho 'Origin' não está presente na solicitação."
                );
            }

            var empresaLocalizada = _companyConnections.List.FirstOrDefault(empresa =>
                empresa.NomeDominio.Contains(domain)
            );

            if(empresaLocalizada == null)
            {
                throw new InvalidOperationException(
                    $"Nenhuma string de conexão encontrada para o domínio: {domain}"
                );
            }
            return empresaLocalizada.ConnnectionString;
        }

        public string IdentificarStringConexao(ActionExecutingContext context)
        {
            var originHeader = context.HttpContext.Request.Headers["Origin"].FirstOrDefault();

            string domain = null;

            if(!string.IsNullOrEmpty(originHeader))
            {
                var originUri = new Uri(originHeader);
                domain = originUri.Host;
            }

            if(string.IsNullOrEmpty(domain))
            {
                context.Result = new BadRequestObjectResult(
                    new ResponseDTO<string>(
                        null,
                        [
                            new Notificacao(
                                "A origem da requisição não pôde ser determinada.",
                                EnumTipoNotificacao.ClientError
                            )
                        ]
                    )
                );
                return null;
            }

            // Buscar a conexão correspondente ao domínio origin
            var empresaLocalizada = _companyConnections.List.FirstOrDefault(empresa =>
                empresa.NomeDominio.Contains(domain)
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
            return empresaLocalizada.ConnnectionString;
        }
    }
}
