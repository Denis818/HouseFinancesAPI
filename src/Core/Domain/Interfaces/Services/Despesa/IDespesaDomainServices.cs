using Domain.Dtos.Despesas.Relatorios;

namespace Domain.Interfaces.Services.Despesa
{
    public interface IDespesaDomainServices
    {
        DistribuicaoCustosMoradiaDto CalcularDistribuicaoCustosMoradia(
            CustosDespesasMoradiaDto custosDespesasMoradia
        );

        DistribuicaoCustosCasaDto CalcularDistribuicaoCustosCasa(
            CustosDespesasCasaDto custosDespesasCasa
        );
    }
}
