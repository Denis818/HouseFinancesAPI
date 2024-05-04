using Application.Interfaces.Services.Despesas;
using Asp.Versioning;
using CasaFinanceiroApi.Attributes.Auth;
using CasaFinanceiroApi.Base;
using Domain.Dtos.Despesas.Criacao;
using Domain.Enumeradores;
using Domain.Models.Despesas;
using Microsoft.AspNetCore.Mvc;

namespace CasaFinanceiroApi.V1.Finance.Despesas
{
    [ApiController]
    [ApiVersion("1")]
    [AutorizationFinance]
    [Route("api/v1/grupo-despesa")]
    public class GrupoDespesaController(
        IServiceProvider service,
        IGrupoDespesaAppService _grupoDespesaServices
    ) : BaseApiController(service)
    {
        #region CRUD
        [HttpGet]
        public async Task<IEnumerable<GrupoDespesa>> GetAllAsync() =>
            await _grupoDespesaServices.GetAllAsync();

        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<GrupoDespesa> PostAsync(GrupoDespesaDto categoriaDto) =>
            await _grupoDespesaServices.InsertAsync(categoriaDto);

        //[HttpPut]
        //[PermissoesFinance(EnumPermissoes.USU_000002)]
        //public async Task<GrupoDespesa> PutAsync(int id, GrupoDespesaDto categoriaDto) =>
        //    await _grupoDespesaServices.UpdateAsync(id, categoriaDto);

        //[HttpDelete]
        //[PermissoesFinance(EnumPermissoes.USU_000003)]
        //public async Task<bool> DeleteAsync(int id) => await _grupoDespesaServices.DeleteAsync(id);
        #endregion
    }
}