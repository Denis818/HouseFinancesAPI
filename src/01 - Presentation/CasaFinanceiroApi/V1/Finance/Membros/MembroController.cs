using Application.Interfaces.Services.Membros;
using Asp.Versioning;
using CasaFinanceiroApi.Attributes.Auth;
using CasaFinanceiroApi.Base;
using Domain.Dtos.Membros;
using Domain.Enumeradores;
using Domain.Models.Membros;
using Microsoft.AspNetCore.Mvc;

namespace CasaFinanceiroApi.V1.Finance.Membros
{
    [ApiController]
    [ApiVersion("1")]
    [AutorizationFinance]
    [Route("api/v1/membro")]
    public class MembroController(IServiceProvider service, IMembroAppServices _membroServices)
        : BaseApiController(service)
    {
        #region CRUD
        [HttpGet]
        public async Task<IEnumerable<Membro>> GetAllDespesaAsync() =>
            await _membroServices.GetAllAsync();

        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Membro> Post(MembroDto vendaDto) =>
            await _membroServices.InsertAsync(vendaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<Membro> Put(int id, MembroDto vendaDto) =>
            await _membroServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task<bool> Delete(int id) => await _membroServices.DeleteAsync(id);
        #endregion

        [HttpGet("enviar-mensagem")]
        public async Task<object> EnviarValoresDividosPeloWhatsAppAsync(
            string nome,
            string pix,
            bool isMoradia,
            string titleMessage
        )
        {
            var messagemWhastApp = await _membroServices.EnviarValoresDividosPeloWhatsAppAsync(
                nome,
                titleMessage,
                isMoradia,
                pix
            );

            return new { RedirectToWhatsApp = messagemWhastApp };
        }
    }
}
