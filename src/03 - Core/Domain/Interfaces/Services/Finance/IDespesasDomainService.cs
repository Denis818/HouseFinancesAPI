using Domain.Dtos.Categoria;
using Domain.Dtos.Finance;
using Domain.Models.Finance;

namespace Domain.Interfaces.Services.Finance
{
    public interface IDespesasDomainService
    {
        (double, double) CalculaTotalAlmocoDivididoComJhon(
            List<Despesa> despesas,
            int idAlmoco,
            int todosMembros
        );
        double CalculaTotalDespesaForaAlmocoAluguelCondominioContaDeLuz(
            List<Despesa> despesas,
            CategoriaIdsDto categoriaIds
        );
        RelatorioGastosDoMesDto GetRelatorioDeGastosDoMes(
            string mesAtual,
            CategoriaIdsDto categIds,
            List<Despesa> despesas
        );
        (double, double) CalcularTotalAluguelCondominioContaDeLuzPorMembro(
            List<Despesa> despesas,
            CategoriaIdsDto categoriaIds,
            List<Membro> todosMembros,
            int idJhon,
            int idPeu
        );
    }
}
