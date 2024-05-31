using Domain.Dtos.Despesas.Relatorios;
using Domain.Extensions.Help;
using Domain.Interfaces.Services.Despesa;

namespace Domain.Services
{
    public class DespesaDomainServices : IDespesaDomainServices
    {
        public DistribuicaoCustosMoradiaDto CalcularDistribuicaoCustosMoradia(
            CustosDespesasMoradiaDto custosDespesasMoradia
        )
        {
            double totalAptoMaisCaixa =
                custosDespesasMoradia.ParcelaApartamento + custosDespesasMoradia.ParcelaCaixa;

            double totalLuzMaisCondominio =
                custosDespesasMoradia.ContaDeLuz + custosDespesasMoradia.Condominio;

            double totalAptoMaisCaixaAbate300Peu100Estacionamento =
               (totalAptoMaisCaixa - 300 - 100).RountToZeroIfNegative(); //300 aluguel cobrado do peu. 100 reais do estacionamento alugado.

            double valorAptoMaisCaixaParaCadaMembro = 
                (totalAptoMaisCaixaAbate300Peu100Estacionamento 
                    / custosDespesasMoradia.MembrosForaJhonPeuCount).RountToZeroIfNegative();
                
            double valorLuzMaisCondominioParaCadaMembro =
                (totalLuzMaisCondominio / custosDespesasMoradia.MembrosForaJhonCount).RountToZeroIfNegative();
            

            double valorParaMembrosForaPeu =
                valorAptoMaisCaixaParaCadaMembro + valorLuzMaisCondominioParaCadaMembro;

            double valorParaDoPeu = 300 + valorLuzMaisCondominioParaCadaMembro;

            return new DistribuicaoCustosMoradiaDto()
            {
                ValorParaDoPeu = valorParaDoPeu,
                TotalAptoMaisCaixa = totalAptoMaisCaixa,
                TotalLuzMaisCondominio = totalLuzMaisCondominio,

                ValorParaMembrosForaPeu = 
                    valorParaMembrosForaPeu.RountToZeroIfNegative(),

                ValorAptoMaisCaixaParaCadaMembro = 
                    valorAptoMaisCaixaParaCadaMembro.RountToZeroIfNegative(),

                TotalAptoMaisCaixaAbate300Peu100Estacionamento = 
                    totalAptoMaisCaixaAbate300Peu100Estacionamento.RountToZeroIfNegative(),

                ValorLuzMaisCondominioParaCadaMembro = 
                    valorLuzMaisCondominioParaCadaMembro.RountToZeroIfNegative(),
            };
        }

        public DistribuicaoCustosCasaDto CalcularDistribuicaoCustosCasa(
            CustosDespesasCasaDto custosDespesasCasa)
        {
            //  Almoço divido com Jhon
            double totalAlmocoParteDoJhon =
                (custosDespesasCasa.ValorTotalAlmoco / custosDespesasCasa.TodosMembros.Count).RountToZeroIfNegative();

            double totalDoAlmocoComAbateParteDoJhon =
                (custosDespesasCasa.ValorTotalAlmoco - totalAlmocoParteDoJhon).RountToZeroIfNegative();

            //Calcular todas as depesas da casa.
            double despesaGeraisMaisAlmoco =
                custosDespesasCasa.TotalDespesaGeraisForaAlmoco
                + custosDespesasCasa.ValorTotalAlmoco;

            //Dividindo as despesas somadas
            double despesaGeraisMaisAlmocoDividioPorMembro =
                (custosDespesasCasa.TotalDespesaGeraisForaAlmoco + totalDoAlmocoComAbateParteDoJhon)
                / custosDespesasCasa.MembrosForaJhonCount;

            return new DistribuicaoCustosCasaDto()
            {
                Membros = custosDespesasCasa.TodosMembros,
                TotalDespesaGeraisForaAlmoco = custosDespesasCasa.TotalDespesaGeraisForaAlmoco,
                TotalAlmocoDividioComJhon = totalDoAlmocoComAbateParteDoJhon,
                TotalAlmocoParteDoJhon = totalAlmocoParteDoJhon,
                DespesaGeraisMaisAlmoco = despesaGeraisMaisAlmoco,
                DespesaGeraisMaisAlmocoDividioPorMembro = despesaGeraisMaisAlmocoDividioPorMembro.RountToZeroIfNegative()
            };
        }
    }
}
