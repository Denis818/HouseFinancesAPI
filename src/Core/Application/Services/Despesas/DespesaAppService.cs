using Application.Interfaces.Services.Despesas;
using Application.Services.Despesas.Base;
using Application.Services.Despesas.RelatorioPdf;
using Domain.Dtos.Despesas.Relatorios;

namespace Application.Services.Despesas
{
    public class DespesaAppService(
        IDespesaCasaAppService _despesaCasaApp,
        IDespesaMoradiaAppService _despesaMoradiaApp,
        IServiceProvider service
    ) : BaseDespesaService(service), IDespesaAppService
    {
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

        public (double, double) CompararFaturaComTotalDeDespesas(double faturaCartao)
        {
            double totalDespesas = ListDespesasPorGrupo
                .Where(despesa =>
                    despesa.CategoriaId != _categoriaIds.IdAluguel
                    && despesa.CategoriaId != _categoriaIds.IdContaDeLuz
                    && despesa.CategoriaId != _categoriaIds.IdCondominio
                )
                .Sum(despesa => despesa.Total);

            double valorSubtraido = totalDespesas - faturaCartao;

            return (totalDespesas, valorSubtraido);
        }

        public async Task<DetalhamentoDespesasMoradiaDto> CalcularDistribuicaoCustosMoradiaAsync() =>
            await _despesaMoradiaApp.CalcularDistribuicaoCustosMoradiaAsync();

        public async Task<DistribuicaoCustosCasaDto> CalcularDistribuicaoCustosCasaAsync() =>
            await _despesaCasaApp.CalcularDistribuicaoCustosCasaAsync();
    }
}
