using Application.Interfaces.Services.Categorias;
using Asp.Versioning;
using CasaFinanceiroApi.Attributes.Auth;
using CasaFinanceiroApi.Base;
using Domain.Dtos.Categorias;
using Domain.Enumeradores;
using Domain.Models.Categorias;
using Microsoft.AspNetCore.Mvc;

namespace CasaFinanceiroApi.V1.Finance
{
    [ApiController]
    [ApiVersion("v1")]
    [AutorizationFinance]
    [Route("api/v1/[controller]")]
    public class CategoriaController(
        IServiceProvider service,
        ICategoriaAppServices _categoriaServices
    ) : BaseApiController(service)
    {
        #region CRUD
        [HttpGet]
        public async Task<IEnumerable<Categoria>> GetAllAsync() =>
            await _categoriaServices.GetAllAsync();

        [HttpPost]
        [PermissoesFinanceAttribute(EnumPermissoes.USU_000001)]
        public async Task<Categoria> PostAsync(CategoriaDto categoriaDto) =>
            await _categoriaServices.InsertAsync(categoriaDto);

        [HttpPut]
        [PermissoesFinanceAttribute(EnumPermissoes.USU_000002)]
        public async Task<Categoria> PutAsync(int id, CategoriaDto categoriaDto) =>
            await _categoriaServices.UpdateAsync(id, categoriaDto);

        [HttpDelete]
        [PermissoesFinanceAttribute(EnumPermissoes.USU_000003)]
        public async Task<bool> DeleteAsync(int id) => await _categoriaServices.DeleteAsync(id);
        #endregion
    }
}

namespace CasaFinanceiroApi.V2.Finance
{
    [ApiVersion("v2")]
    [Route("[controller]")]
    [Route("api/v2/[controller]")]
    public class V2Controller : ControllerBase
    {
        [HttpGet()]
        public IActionResult Get() => Ok();
    }
}

namespace CasaFinanceiroApi.V3.Finance
{
    [ApiVersion("v3")]
    [Route("[controller]")]
    [Route("api/v3/[controller]")]
    public class V3Controller : ControllerBase
    {
        [HttpGet()]
        public IActionResult Get() => Ok();
    }
}
