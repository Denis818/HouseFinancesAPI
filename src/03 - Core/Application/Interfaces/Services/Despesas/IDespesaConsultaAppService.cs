using Domain.Dtos.Despesas.Consultas;
using Domain.Dtos.Despesas.Relatorios;

namespace Application.Interfaces.Services.Despesas
{
    public interface IDespesaConsultaAppService
    {
        Task<ResumoMensalDto> GetResumoDespesasMensalAsync();
        Task<IEnumerable<DespesasPorMesDto>> GetTotaisComprasPorMesAsync();
        Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoriaAsync();
        Task<DetalhamentoDespesasHabitacionalDto> CalcularDistribuicaoCustosHabitacionalAsync();
    }
}