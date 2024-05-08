using Microsoft.AspNetCore.Mvc.Filters;

namespace DIContainer.DataBaseConfiguration.ConnectionString
{
    public interface IConnectionStringResolver
    {
        string IdentificarStringConexao();
        string IdentificarStringConexao(ActionExecutingContext context);
    }
}
