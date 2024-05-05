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

            double totalAptoMaisCaixaAbate300Peu100Estacionamento = totalAptoMaisCaixa - 300; //300 aluguel cobrado do peu

            double valorAptoMaisCaixaParaCadaMembro =
                (totalAptoMaisCaixaAbate300Peu100Estacionamento - 100) / listMembroForaJhonPeuCount; // 100 reais do estacionamento alugado.

            double valorLuzMaisCondominioParaCadaMembro =
                totalLuzMaisCondominio / listMembroForaJhonCount;

            double valorParaMembrosForaPeu =
                valorAptoMaisCaixaParaCadaMembro + valorLuzMaisCondominioParaCadaMembro;

            double valorParaDoPeu = 300 + valorLuzMaisCondominioParaCadaMembro;

            return new DistribuicaoCustosMoradiaDto()
            {
                ValorParaDoPeu = valorParaDoPeu,
                TotalAptoMaisCaixa = totalAptoMaisCaixa,
                TotalLuzMaisCondominio = totalLuzMaisCondominio,
                ValorParaMembrosForaPeu = Math.Max(valorParaMembrosForaPeu, 0),
                ValorAptoMaisCaixaParaCadaMembro = Math.Max(valorAptoMaisCaixaParaCadaMembro, 0),
                TotalAptoMaisCaixaAbate300Peu100Estacionamento = Math.Max(
                    totalAptoMaisCaixaAbate300Peu100Estacionamento,
                    0
                ),
                ValorLuzMaisCondominioParaCadaMembro = Math.Max(
                    valorLuzMaisCondominioParaCadaMembro,
                    0
                ),
            };
        }
    }
}
