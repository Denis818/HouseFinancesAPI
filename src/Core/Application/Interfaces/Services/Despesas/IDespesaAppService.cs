using Domain.Dtos.Despesas.Consultas;

namespace Application.Interfaces.Services.Despesas
{
    public interface IDespesaAppService
    {
        Task<IEnumerable<DespesasPorGrupoDto>> GetDespesaGrupoParaGraficoAsync();
        Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoriaAsync();
        Task<DespesasDivididasMensalDto> GetAnaliseDesesasPorGrupoAsync();
        Task<byte[]> DownloadPdfRelatorioDeDespesaCasa();
        Task<byte[]> DownloadPdfRelatorioDeDespesaMoradia();
        Task<(double, double)> CompararFaturaComTotalDeDespesas(double faturaCartao);
    }
}
