using Domain.Dtos.Despesas.Relatorios;

namespace Application.Interfaces.Services.Despesas
{
    public interface IDespesaAppService
    {
        Task<DistribuicaoCustosCasaDto> CalcularDistribuicaoCustosCasaAsync();
        Task<DetalhamentoDespesasMoradiaDto> CalcularDistribuicaoCustosMoradiaAsync();
        Task<(double, double)> CompararFaturaComTotalDeDespesas(double faturaCartao);
        Task<byte[]> DownloadPdfRelatorioDeDespesaCasa();
        Task<byte[]> DownloadPdfRelatorioDeDespesaMoradia();
    }
}