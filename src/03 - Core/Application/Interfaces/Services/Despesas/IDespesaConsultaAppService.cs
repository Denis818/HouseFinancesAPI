using Domain.Dtos.Despesas.Consultas;

namespace Application.Interfaces.Services.Despesas
{
    public interface IDespesaConsultaAppService
    {
        Task<IEnumerable<DespesasPorGrupoDto>> GetDespesaGrupoParaGraficoAsync();
        Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoriaAsync();
        Task<DespesasDivididasMensalDto> GetAnaliseDesesasPorGrupoAsync();
    }
}
