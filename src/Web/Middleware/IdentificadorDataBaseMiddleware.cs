using Data.DataContext;
using Infraestructure.Data.Configurations;

namespace Web.Middleware
{
    public class IdentificadorDataBaseMiddleware(
        FinanceDbContext dbContext,
        CompanyConnectionStrings companyConnections
    ) : IMiddleware
    {
        private readonly FinanceDbContext _context = dbContext;
        private readonly CompanyConnectionStrings _companyConnections = companyConnections;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var connectionString = IdentificarStringConexao(context);

            _context.SetConnectionString(connectionString);

            await next(context);
        }

        private string IdentificarStringConexao(HttpContext context)
        {
            string origin = context.Request.Headers["Origin"].ToString();

            string hostName = null;

            if (string.IsNullOrEmpty(origin))
            {
                origin = $"https://{context.Request.Host}";
            }

            if (!string.IsNullOrEmpty(origin))
            {
                var originUri = new Uri(origin);
                hostName = originUri.Host;
            }

            var empresaLocalizada = _companyConnections.List.FirstOrDefault(empresa =>
                empresa.NomeDominio == hostName
            );

            if (empresaLocalizada == null)
            {
                throw new Exception($"A empresa com nome de domínio '{hostName}' não existe");
            }

            return empresaLocalizada.ConnectionString;
        }
    }
}
