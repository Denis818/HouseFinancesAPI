namespace Application.Interfaces.Services.Despesas
{
    public interface IDespesaAppService
    {
        (double, double) CompararFaturaComTotalDeDespesas(double faturaCartao);
        Task<byte[]> DownloadPdfRelatorioDeDespesaCasa();
        Task<byte[]> DownloadPdfRelatorioDeDespesaMoradia();
    }
}
