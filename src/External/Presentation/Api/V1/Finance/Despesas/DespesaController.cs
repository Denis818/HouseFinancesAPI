using System.Net.Mime;
using Application.Interfaces.Services.Despesas;
using Asp.Versioning;
using Domain.Dtos.Despesas.Consultas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Api.Base;
using Presentation.Attributes.Auth;
using Presentation.Attributes.Util;
using Presentation.Version;

namespace Presentation.Api.V1.Finance.Despesas
{
    [ApiController]
    [ApiVersion(ApiVersioning.V1)]
    [AutorizationFinance]
    [GetIdGroupInHeaderFilter]
    [Route("api/v1/despesa")]
    public class DespesaController(IServiceProvider service, IDespesaAppService _despesaAppService)
        : MainController(service)
    {
        #region Consultas
        [HttpGet("analise-despesa-por-grupo")]
        public async Task<DespesasDivididasMensalDto> GetAnaliseDesesasPorGrupoAsync() =>
            await _despesaAppService.GetAnaliseDesesasPorGrupoAsync();

        [HttpGet("total-por-categoria")]
        public async Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoriaAsync() =>
            await _despesaAppService.GetTotalPorCategoriaAsync();

        [HttpGet("total-por-grupo")]
        public async Task<IEnumerable<DespesasPorGrupoDto>> GetDespesaGrupoParaGraficoAsync() =>
            await _despesaAppService.GetDespesaGrupoParaGraficoAsync();
        #endregion

        #region Downloads
        [HttpGet("pdf-despesas-casa")]
        public async Task<FileContentResult> DownloadCalculoCasa()
        {
            byte[] pdfBytes = await _despesaAppService.DownloadPdfRelatorioDeDespesaCasa();

            var contentDisposition = new ContentDisposition
            {
                FileName = "relatorio-despesas-casa.pdf",
                Inline = false
            };

            Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            return File(pdfBytes, "application/pdf");
        }

        [HttpGet("pdf-despesas-moradia")]
        public async Task<FileContentResult> DownloadCalculoMoradia()
        {
            byte[] pdfBytes = await _despesaAppService.DownloadPdfRelatorioDeDespesaMoradia();

            var contentDisposition = new ContentDisposition
            {
                FileName = "relatorio-despesas-Moradia.pdf",
                Inline = false
            };

            Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            return File(pdfBytes, "application/pdf");
        }
        #endregion

        [HttpGet("calcular-fatura")]
        public async Task<object> ConferirFaturaDoCartao(double faturaCartao)
        {
            (double totalDespesas, double valorSubtraido) =
                await _despesaAppService.CompararFaturaComTotalDeDespesas(faturaCartao);

            return new { TotalDespesa = totalDespesas, ValorSubtraido = valorSubtraido };
        }
    }
}
