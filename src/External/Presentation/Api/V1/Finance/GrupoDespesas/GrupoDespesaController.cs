using Application.Interfaces.Services.Despesas;
using Asp.Versioning;
using Domain.Dtos.Despesas.Criacao;
using Domain.Enumeradores;
using Domain.Models.Despesas;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Base;
using Presentation.Attributes.Auth;
using Presentation.Version;

namespace Presentation.Api.V1.Finance.GrupoFaturas
{
    [ApiController]
    [ApiVersion(ApiVersioning.V1)]
    [AutorizationFinance]
    [Route("api/v1/grupo-despesa")]
    public class GrupoFaturaController(
        IServiceProvider service,
        IGrupoFaturaAppService _GrupoFaturaServices
    ) : MainController(service)
    {
        #region CRUD
        [HttpGet]
        public async Task<IEnumerable<GrupoFatura>> GetAllAsync() =>
            await _GrupoFaturaServices.GetAllAsync();

        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<GrupoFatura> PostAsync(GrupoFaturaDto categoriaDto) =>
            await _GrupoFaturaServices.InsertAsync(categoriaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<GrupoFatura> PutAsync(int id, GrupoFaturaDto categoriaDto) =>
            await _GrupoFaturaServices.UpdateAsync(id, categoriaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task<bool> DeleteAsync(int id) => await _GrupoFaturaServices.DeleteAsync(id);
        #endregion
    }
}
