using Domain.Dtos.Despesas.Relatorios;

namespace Domain.Interfaces.Services.Despesa
{
    public interface IDespesaDomainServices
    {
        DistribuicaoCustosMoradiaDto CalcularDistribuicaoCustosMoradiaAsync(
            double parcelaApartamento,
            double parcelaCaixa,
            double contaDeLuzValue,
            double condominioValue,
            int listMembroForaJhonPeuCount,
            int listMembroForaJhonCount
        );
    }
}
