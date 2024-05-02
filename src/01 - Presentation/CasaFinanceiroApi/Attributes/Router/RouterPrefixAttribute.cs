using Microsoft.AspNetCore.Mvc;

namespace CasaFinanceiroApi.Attributes.Router
{
    public class RouterPrefixAttribute(string version, string template)
        : RouteAttribute($"api/{version}/{template}")
    { }
}
