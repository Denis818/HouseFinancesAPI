using Application.Interfaces.Services.Despesas;
using Application.Utilities;
using Domain.Dtos.Despesas.Consultas;
using Domain.Dtos.Despesas.Criacao;
using Domain.Enumeradores;
using Domain.Models.Despesas;
using HouseFinancesAPI.Attributes;
using HouseFinancesAPI.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace HouseFinancesAPI.Controllers.Finance
{
    [ApiController]
    // [AutorizationFinance]
    [Route("api/[controller]")]
    public class DespesaController(IServiceProvider service, IDespesaAppServices _despesaServices, IDespesaConsultaAppService _despesaConsultaApp)
        : BaseApiController(service)
    {
        #region CRUD
        [HttpGet]
        public async Task<PagedResult<Despesa>> GetAllAsync(
            int paginaAtual = 1,
            int itensPorPagina = 10
        ) => await _despesaServices.GetAllAsync(paginaAtual, itensPorPagina);

        [HttpPost]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Despesa> PostAsync(DespesaDto vendaDto) =>
            await _despesaServices.InsertAsync(vendaDto);

        [HttpPut]
        [PermissoesFinance(EnumPermissoes.USU_000002)]
        public async Task<Despesa> PutAsync(int id, DespesaDto vendaDto) =>
            await _despesaServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000003)]
        public async Task<bool> DeleteAsync(int id) => await _despesaServices.DeleteAsync(id);
        #endregion

        #region Consultas
        [HttpGet("resumo-despesas-mensal")]
        public async Task<ResumoMensalDto> GetResumoDespesasMensalAsync() =>
            await _despesaConsultaApp.GetResumoDespesasMensalAsync();

        [HttpGet("total-por-categoria")]
        public async Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoriaAsync() =>
            await _despesaConsultaApp.GetTotalPorCategoriaAsync();

        [HttpGet("total-por-mes")]
        public async Task<IEnumerable<DespesasPorMesDto>> GetTotaisComprasPorMesAsync() =>
            await _despesaConsultaApp.GetTotaisComprasPorMesAsync();

        [HttpGet("pdf-despesas-habitacional")]
        public async Task<FileContentResult> DownloadCalculoHabitacional()
        {
            byte[] pdfBytes = await _despesaServices.DownloadPdfRelatorioDeDespesaHabitacional();

            var contentDisposition = new ContentDisposition
            {
                FileName = "relatorio-despesas-habitacional.pdf",
                Inline = false
            };

            Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            return File(pdfBytes, "application/pdf");
        }

        #endregion

        [HttpPost("inserir-lote")]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<IEnumerable<Despesa>> PostRangeAsync(
            IAsyncEnumerable<DespesaDto> vendaDto
        ) => await _despesaServices.InsertRangeAsync(vendaDto);


        [HttpGet("pdf-despesas-casa")]
        public async Task<FileContentResult> DownloadCalculoCasa()
        {
            byte[] pdfBytes = await _despesaServices.DownloadPdfRelatorioDeDespesaCasa();

            var contentDisposition = new ContentDisposition
            {
                FileName = "relatorio-despesas-casa.pdf",
                Inline = false
            };

            Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            return File(pdfBytes, "application/pdf");
        }
    }
}
