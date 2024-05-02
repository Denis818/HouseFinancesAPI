using Application.Interfaces.Services.Categorias;
using Domain.Dtos.Categorias;
using Domain.Enumeradores;
using Domain.Models.Categorias;
using CasaFinanceiroApi.Attributes;
using CasaFinanceiroApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace CasaFinanceiroApi.Controllers.Finance
{
    [ApiController]
    [AutorizationFinance]
    [Route("api/[controller]")]
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
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Categoria> PostAsync(CategoriaDto categoriaDto) =>
            await _categoriaServices.InsertAsync(categoriaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<Categoria> PutAsync(int id, CategoriaDto categoriaDto) =>
            await _categoriaServices.UpdateAsync(id, categoriaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task<bool> DeleteAsync(int id) => await _categoriaServices.DeleteAsync(id);
        #endregion
    }
}
