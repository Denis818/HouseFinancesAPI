using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Base;
using Presentation.Version;

namespace Presentation.Api.V2
{
    [ApiController]
    [ApiVersion(ApiVersioning.V2)]
    [Route("api/v2/produto")]
    public class ProdutoController : MainController
    {
        public ProdutoController(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        [HttpGet]
        public IEnumerable<object> GetAsync()
        {
            return new object[] { };
        }
    }
}
