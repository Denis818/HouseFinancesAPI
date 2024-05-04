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
                ValorParaMembrosForaPeu = Math.Max(valorParaMembrosForaPeu, 0),
                TotalAptoMaisCaixaAbate300Peu = Math.Max(totalAptoMaisCaixaAbate300Peu, 0),
                ValorAptoMaisCaixaParaCadaMembro = Math.Max(valorAptoMaisCaixaParaCadaMembro, 0),
                ValorLuzMaisCondominioParaCadaMembro = Math.Max(
                    valorLuzMaisCondominioParaCadaMembro,
                    0
                ),
                TotalLuzMaisCondominioAbate100Estacionamento = Math.Max(
                    totalLuzMaisCondominioAbate100Estacionamento,
                    0
                ),
            };
        }
    }
}
