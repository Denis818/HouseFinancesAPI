using Application.Interfaces.Services.Despesas;
using Asp.Versioning;
using CasaFinanceiroApi.Attributes.Auth;
using CasaFinanceiroApi.Base;
using CasaFinanceiroApi.Filters;
using Microsoft.AspNetCore.Mvc;
using Save.Cache.Memory;
using System.Net.Mime;

namespace CasaFinanceiroApi.V1.Finance.Despesas
{
    [Cached]
    [ApiController]
    [ApiVersion("1")]
    [AutorizationFinance]
    [GetIdGroupInHeaderFilter]
    [Route("api/v1/despesa")]
    public class DespesaController(IServiceProvider service, IDespesaAppService _despesaServices)
        : BaseApiController(service)
    {
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

        [HttpGet("pdf-despesas-moradia")]
        public async Task<FileContentResult> DownloadCalculoMoradia()
        {
            byte[] pdfBytes = await _despesaServices.DownloadPdfRelatorioDeDespesaMoradia();

            var contentDisposition = new ContentDisposition
            {
                FileName = "relatorio-despesas-Moradia.pdf",
                Inline = false
            };

            Response.Headers.Append("Content-Disposition", contentDisposition.ToString());

            return File(pdfBytes, "application/pdf");
        }

        [HttpGet("calcular-fatura")]
        public async Task<object> ConferirFaturaDoCartao(double faturaCartao)
        {
            (double totalDespesas, double valorSubtraido) =
                await _despesaServices.CompararFaturaComTotalDeDespesas(faturaCartao);

            return new { TotalDespesa = totalDespesas, ValorSubtraido = valorSubtraido };
        }
    }
}
