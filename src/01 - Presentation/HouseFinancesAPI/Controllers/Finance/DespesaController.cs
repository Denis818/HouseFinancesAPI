using Application.Interfaces.Services.Finance;
using Application.Utilities;
using Domain.Dtos.Finance;
using Domain.Enumeradores;
using Domain.Models.Finance;
using HouseFinancesAPI.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace HouseFinancesAPI.Controllers.Finance
{
    [ApiController]
    // [AutorizationFinance]
    [Route("api/[controller]")]
    public class DespesaController(IServiceProvider service, IDespesaAppServices _despesaServices) : Controller

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
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<Despesa> PutAsync(int id, DespesaDto vendaDto) =>
            await _despesaServices.UpdateAsync(id, vendaDto);

        [HttpDelete]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task DeleteAsync(int id) => await _despesaServices.DeleteAsync(id);
        #endregion


        [HttpPost("inserir-lote")]
        [PermissoesFinance(EnumPermissoes.USU_000001)]
        public async Task<IEnumerable<Despesa>> PostRangeAsync(IAsyncEnumerable<DespesaDto> vendaDto) =>
            await _despesaServices.InsertRangeAsync(vendaDto);

        [HttpGet("resumo-despesas-mensal")]
        public async Task<ResumoMensalDto> GetResumoDespesasMensalAsync() =>
            await _despesaServices.GetResumoDespesasMensalAsync();

        [HttpGet("total-por-categoria")]
        public async Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoriaAsync() =>
            await _despesaServices.GetTotalPorCategoriaAsync();

        [HttpGet("total-por-mes")]
        public async Task<IEnumerable<DespesasPorMesDto>> GetTotaisComprasPorMesAsync() =>
            await _despesaServices.GetTotaisComprasPorMesAsync();

        [HttpGet("gerar-pdf")]
        public IActionResult fileContentResult()
        {
            // Gerar o PDF
            byte[] pdfBytes = _despesaServices.PdfValoresAluguelCondominioLuz();

            // Configurar o cabeçalho Content-Disposition para download
            var contentDisposition = new ContentDisposition
            {
                FileName = "arquivo.pdf",
                Inline = false  // Define como false para forçar o download
            };
            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

            // Retornar o PDF como um FileContentResult
            return File(pdfBytes, "application/pdf");
        }
    }
}
