using Domain.Dtos.Despesas.Consultas;

namespace Application.Interfaces.Services.Despesas
{
    public interface IDespesaConsultaAppService
    {
        Task<IEnumerable<DespesasPorGrupoDto>> GetDespesaGrupoParaGraficoAsync();
        IEnumerable<DespesasTotalPorCategoria> GetTotalPorCategoria();
        Task<DespesasDivididasMensalDto> GetAnaliseDesesasPorGrupoAsync();
    }
}
