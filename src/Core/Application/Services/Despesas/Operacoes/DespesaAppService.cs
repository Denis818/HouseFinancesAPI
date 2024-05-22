using Application.Interfaces.Services.Despesas;
using Application.Services.Despesas.Base;
using Application.Services.Despesas.RelatorioPdf;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Despesas.Operacoes
{
    public class DespesaAppService(
        IServiceProvider service,
        IDespesaCasaAppService _despesaCasaApp,
        IDespesaMoradiaAppService _despesaMoradiaApp
    ) : BaseDespesaService(service), IDespesaAppService
    {
        #region Downloads
        public async Task<byte[]> DownloadPdfRelatorioDeDespesaCasa()
        {
            var custosCasaDto = await _despesaCasaApp.CalcularDistribuicaoCustosCasaAsync();

            return new DespesaCasaPdfReport().GerarRelatorioDespesaCasaPdf(custosCasaDto);
        }

        public async Task<byte[]> DownloadPdfRelatorioDeDespesaMoradia()
        {
            var custosMoradiaDto =
                await _despesaMoradiaApp.CalcularDistribuicaoCustosMoradiaAsync();

            return new DespesaMoradiaPdfReport().GerarRelatorioDespesaMoradiaPdf(custosMoradiaDto);
        }
        #endregion

        #region Utilitários
        public async Task<(double, double)> CompararFaturaComTotalDeDespesas(double faturaCartao)
        {
            double totalDespesas = await _queryDespesasPorGrupo
                .Where(despesa =>
                    despesa.CategoriaId != _categoriaIds.IdAluguel
                    && despesa.CategoriaId != _categoriaIds.IdContaDeLuz
                    && despesa.CategoriaId != _categoriaIds.IdCondominio
                )
                .SumAsync(despesa => despesa.Total);

            double valorSubtraido = totalDespesas - faturaCartao;

            return (totalDespesas, valorSubtraido);
        }
        #endregion
    }
}
