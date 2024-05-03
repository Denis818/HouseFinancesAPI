using Domain.Dtos.Despesas.Relatorios;
using Domain.Interfaces.Services.Despesa;

namespace Domain.Services
{
    public class DespesaDomainServices : IDespesaDomainServices
    {
        public DistribuicaoCustosMoradiaDto CalcularDistribuicaoCustosMoradiaAsync(
            double parcelaApartamento,
            double parcelaCaixa,
            double contaDeLuzValue,
            double condominioValue,
            int listMembroForaJhonPeuCount,
            int listMembroForaJhonCount
        )
        {
            double totalAptoMaisCaixa = parcelaApartamento + parcelaCaixa;
            double totalLuzMaisCondominio = contaDeLuzValue + condominioValue;

            double totalAptoMaisCaixaAbate300Peu = totalAptoMaisCaixa - 300; //300 aluguel cobrado do peu
            double totalLuzMaisCondominioAbate100Estacionamento = totalLuzMaisCondominio - 100; //estacionamento alugado

            double valorAptoMaisCaixaParaCadaMembro =
                totalAptoMaisCaixaAbate300Peu / listMembroForaJhonPeuCount;

            double valorLuzMaisCondominioParaCadaMembro =
                totalLuzMaisCondominioAbate100Estacionamento / listMembroForaJhonCount;

            double valorParaMembrosForaPeu =
                valorAptoMaisCaixaParaCadaMembro + valorLuzMaisCondominioParaCadaMembro;

            double valorParaDoPeu = 300 + valorLuzMaisCondominioParaCadaMembro;

            return new DistribuicaoCustosMoradiaDto()
            {
                ValorParaDoPeu = valorParaDoPeu,
                TotalAptoMaisCaixa = totalAptoMaisCaixa,
                TotalLuzMaisCondominio = totalLuzMaisCondominio,
                ValorParaMembrosForaPeu = valorParaMembrosForaPeu,
                TotalAptoMaisCaixaAbate300Peu = totalAptoMaisCaixaAbate300Peu,
                ValorAptoMaisCaixaParaCadaMembro = valorAptoMaisCaixaParaCadaMembro,
                ValorLuzMaisCondominioParaCadaMembro = valorLuzMaisCondominioParaCadaMembro,
                TotalLuzMaisCondominioAbate100Estacionamento =
                    totalLuzMaisCondominioAbate100Estacionamento,
            };
        }
    }
}
