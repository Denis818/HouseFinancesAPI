using Domain.Dtos.Finance;
using Domain.Models.Finance;

namespace Domain.Services
{
    public class FinanceServices : IFinanceServices
    {
        public double CalculaTotalDespesaForaAlmocoAluguel(
            List<Despesa> despesas,
            int idAlmoco,
            int idAluguel
        )
        {
            return despesas
                .Where(d => d.CategoriaId != idAlmoco && d.CategoriaId != idAluguel)
                .Sum(d => d.Total);
        }

        public RelatorioGastosDoMesDto GetRelatorioDeGastosDoMes(
            string mesAtual,
            List<Despesa> despesas
        )
        {
            double totalAluguelMaisCondominio = despesas
                .Where(d =>
                    d.Categoria.Descricao == "Aluguel" || d.Categoria.Descricao == "Condomínio"
                )
                .Sum(d => d.Total);

            double totalGeral = despesas.Sum(d => d.Total);

            double totalGastosGerais = totalGeral - totalAluguelMaisCondominio;

            return new RelatorioGastosDoMesDto(
                mesAtual,
                totalAluguelMaisCondominio,
                totalGastosGerais,
                totalGeral
            );
        }

        public (double, double) CalculaTotalAlmocoDivididoComJhon(
            List<Despesa> despesas,
            int idAlmoco
        )
        {
            var totalAlmoco = despesas.Where(d => d.CategoriaId == idAlmoco).Sum(d => d.Total);

            double totalAlmocoParteDoJhon = (totalAlmoco / 5);

            double totalAlmocoAbatido = (totalAlmoco - totalAlmocoParteDoJhon);

            return (totalAlmocoAbatido, totalAlmocoParteDoJhon);
        }
    }
}
