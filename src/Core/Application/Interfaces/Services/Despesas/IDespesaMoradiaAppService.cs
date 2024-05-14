using Domain.Dtos.Despesas.Relatorios;

namespace Application.Interfaces.Services.Despesas
{
    public interface IDespesaMoradiaAppService
    {
        Task<DetalhamentoDespesasMoradiaDto> CalcularDistribuicaoCustosMoradiaAsync();
    }
}