using Domain.Dtos.Finance;
using Domain.Models.Finance;

namespace Domain.Services
{
    public class FinanceServices : IFinanceServices
    {
        public decimal CalculaTotalDespesaForaAlmocoAluguel(List<Despesa> despesas, int idAlmoco, int idAluguel)
        {
            return despesas.Where(d => d.CategoriaId != idAlmoco &&
                                       d.CategoriaId != idAluguel).Sum(d => d.Total);
        }

        public RelatorioGastosDoMesDto GetRelatorioDeGastosDoMes(string mesAtual, List<Despesa> despesas)
        {
            decimal totalAluguelMaisCondominio = despesas
                                                 .Where(d => d.Categoria.Descricao == "Aluguel" ||
                                                             d.Categoria.Descricao == "Condomínio")
                                                 .Sum(d => d.Total);

            decimal totalGeral = despesas.Sum(d => d.Total);

            decimal totalGastosGerais = totalGeral - totalAluguelMaisCondominio;

            return new RelatorioGastosDoMesDto(mesAtual, totalAluguelMaisCondominio, totalGastosGerais, totalGeral);
        }

        public (decimal, decimal) CalculaTotalAlmocoDivididoComJhon(
            List<Despesa> despesas,
            int idAlmoco)
        {
            var totalAlmoco = despesas.Where(d => d.CategoriaId == idAlmoco)
                                      .Sum(d => d.Total);

            decimal totalAlmocoParteDoJhon = (totalAlmoco / 5);

            decimal totalAlmocoAbatido = (totalAlmoco - totalAlmocoParteDoJhon);

            return (totalAlmocoAbatido, totalAlmocoParteDoJhon);
        }
    }
}
