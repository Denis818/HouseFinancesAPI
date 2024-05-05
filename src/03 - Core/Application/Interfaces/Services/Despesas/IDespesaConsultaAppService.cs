using Domain.Dtos.Despesas.Consultas;

namespace Application.Interfaces.Services.Despesas
{
    public interface IDespesaConsultaAppService
    {
        Task<IEnumerable<DespesasPorMesDto>> GetDespesaGrupoParaGraficoAsync();
        Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoriaAsync();
        Task<DespesasDivididasMensalDto> GetAnaliseDesesasPorGrupoAsync();
    }
}
