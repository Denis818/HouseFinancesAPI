using Domain.Dtos.Despesas.Relatorios;

namespace Application.Interfaces.Services.Despesas
{
    public interface IDespesaCasaAppService
    {
        Task<DistribuicaoCustosCasaDto> CalcularDistribuicaoCustosCasaAsync();
    }
}