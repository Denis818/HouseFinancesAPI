using Application.Interfaces.Services.Despesas;
using Application.Utilities;
using Asp.Versioning;
using CasaFinanceiroApi.Attributes.Auth;
using CasaFinanceiroApi.Base;
using Domain.Dtos.Despesas.Consultas;
using Domain.Dtos.Despesas.Criacao;
using Domain.Enumeradores;
using Domain.Models.Despesas;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace CasaFinanceiroApi.V1.Finance
{
    [ApiController]
    [ApiVersion("v1")]
    [AutorizationFinance]
    [Route("api/v1/[controller]")]
    public class DespesaController(
        IServiceProvider service,
        IDespesaAppServices _despesaServices,
        IDespesaConsultaAppService _despesaConsultaApp
    ) : BaseApiController(service)
    {
        #region CRUD
        [HttpGet]
        public async Task<PagedResult<Despesa>> GetAllAsync(
            int paginaAtual = 1,
            int itensPorPagina = 10
        ) => await _despesaServices.GetAllAsync(paginaAtual, itensPorPagina);

        [HttpPost]
        [PermissoesFinanceAttribute(EnumPermissoes.USU_000001)]
        public async Task<Despesa> PostAsync(DespesaDto vendaDto) =>
            await _despesaServices.InsertAsync(vendaDto);

        [HttpPut]
        [PermissoesFinanceAttribute(EnumPermissoes.USU_000002)]
        public async Task<Despesa> PutAsync(int id, DespesaDto vendaDto) =>
            await _despesaServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
        [PermissoesFinanceAttribute(EnumPermissoes.USU_000003)]
        public async Task<bool> DeleteAsync(int id) => await _despesaServices.DeleteAsync(id);

        [HttpPost("inserir-lote")]
        [PermissoesFinanceAttribute(EnumPermissoes.USU_000001)]
        public async Task<IEnumerable<Despesa>> PostRangeAsync(
            IAsyncEnumerable<DespesaDto> vendaDto
        ) => await _despesaServices.InsertRangeAsync(vendaDto);
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
        #endregion

        #region Downloads
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
        #endregion

        [HttpGet("calcular-fatura")]
        public async Task<object> ConferirFaturaDoCartao(double faturaCartao)
        {
            (double totalDespesas, double valorSubtraido) =
                await _despesaConsultaApp.ConferirFaturaDoCartaoComDespesasAsync(faturaCartao);

            return new { TotalDespesa = totalDespesas, ValorSubtraido = valorSubtraido };
        }
    }
}
