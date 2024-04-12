using Application.Interfaces.Services.Finance;
using Domain.Dtos.Categoria;
using Domain.Enumeradores;
using Domain.Models.Finance;
using HouseFinancesAPI.Attributes;
using HouseFinancesAPI.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HouseFinancesAPI.Controllers.Finance
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

        [HttpGet("{id}")]
        public async Task<Categoria> GetById(int id) => await _categoriaServices.GetByIdAsync(id);

        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Categoria> Post(CategoriaDto vendaDto) =>
            await _categoriaServices.InsertAsync(vendaDto);

        [HttpPatch]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Categoria> Put(int id, CategoriaDto vendaDto) =>
            await _categoriaServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task Delete(int id) => await _categoriaServices.DeleteAsync(id);
        #endregion
    }
}
