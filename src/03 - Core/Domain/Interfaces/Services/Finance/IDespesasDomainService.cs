using Domain.Dtos.Categoria;
using Domain.Dtos.Finance;
using Domain.Models.Finance;

namespace Domain.Interfaces.Services.Finance
{
    public interface IDespesasDomainService
    {
        (double, double) CalculaTotalAlmocoDivididoComJhon(List<Despesa> despesas, int idAlmoco, int todosMembros);
        double CalculaTotalDespesaForaAlmocoAluguelCondominioContaDeLuz(List<Despesa> despesas, CategoriaIdsDto categoriaIds);
        RelatorioGastosDoMesDto GetRelatorioDeGastosDoMes(string mesAtual, CategoriaIdsDto categIds, List<Despesa> despesas);
        byte[] PdfValoresAluguelCondominioLuz(
            List<Despesa> despesas,
            List<Membro> membros,
            CategoriaIdsDto categoriaIds,
            int idJhon,
            int idPeu
        );
    }
}