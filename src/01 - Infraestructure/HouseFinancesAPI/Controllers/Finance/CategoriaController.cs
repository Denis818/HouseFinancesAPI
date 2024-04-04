using Domain.Enumeradores;
using Domain.Models;
using HouseFinancesAPI.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HouseFinancesAPI.Controllers.Base;
using Application.Interfaces.Services.Finance;
using Domain.Dtos.Finance.Records;

namespace HouseFinancesAPI.Controllers.Finance
{
    [ApiController]
    [AutorizationFinance]
    [Route("api/[controller]")]
    public class CategoriaController(IServiceProvider service,
        ICategoriaServices _categoriaServices) :
        BaseApiController(service)
    {
        #region CRUD
        [HttpGet]
        public async Task<IEnumerable<Categoria>> GetAllAsync() =>
            await _categoriaServices.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<Categoria> GetById(int id)
           => await _categoriaServices.GetByIdAsync(id);

        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Categoria> Post(CategoriaDto vendaDto)
            => await _categoriaServices.InsertAsync(vendaDto);

        [HttpPatch]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Categoria> Put(int id, CategoriaDto vendaDto)
            => await _categoriaServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task Delete(int id)
            => await _categoriaServices.DeleteAsync(id);
        #endregion
    }
}
