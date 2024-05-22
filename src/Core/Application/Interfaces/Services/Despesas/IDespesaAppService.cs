namespace Application.Interfaces.Services.Despesas
{
    public interface IDespesaAppService
    {
        Task<byte[]> DownloadPdfRelatorioDeDespesaCasa();
        Task<byte[]> DownloadPdfRelatorioDeDespesaMoradia();
        Task<(double, double)> CompararFaturaComTotalDeDespesas(double faturaCartao);
    }
}
