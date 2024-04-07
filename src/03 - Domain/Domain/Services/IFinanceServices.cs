using Domain.Dtos.Finance;
using Domain.Models.Finance;

namespace Domain.Services
{
    public interface IFinanceServices
    {
        decimal CalculaTotalDespesaForaAlmocoAluguel(List<Despesa> despesas, int idAlmoco, int idAluguel);
        RelatorioGastosDoMesDto GetRelatorioDeGastosDoMes(string mesAtual, List<Despesa> despesas);
        (decimal, decimal) CalculaTotalAlmocoDivididoComJhon(List<Despesa> despesas, int idAlmoco);
    }
}