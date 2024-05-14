using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Presentation.Base;
using Presentation.Configurations.Extensions;

namespace Presentation.V2
{
    [ApiController]
    [ApiVersion(ApiConfig.V2)]
    [Route("api/v2/categoria")]
    public class ProdutoController : MainController
    {
        public ProdutoController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet]
        public IEnumerable<object> GetAsync()
        {
            return new object[]
            {

            };
        }
    }
}
