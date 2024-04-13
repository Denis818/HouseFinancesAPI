using Domain.Dtos.Finance;
using Domain.Models.Finance;

namespace Domain.Services
{
    public interface IFinanceServices
    {
        double CalculaTotalDespesaForaAlmocoAluguel(
            List<Despesa> despesas,
            int idAlmoco,
            int idAluguel
        );
        RelatorioGastosDoMesDto GetRelatorioDeGastosDoMes(string mesAtual, List<Despesa> despesas);
        (double, double) CalculaTotalAlmocoDivididoComJhon(List<Despesa> despesas, int idAlmoco);
    }
}
