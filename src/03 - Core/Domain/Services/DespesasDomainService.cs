using Domain.Dtos.Categoria;
using Domain.Dtos.Finance;
using Domain.Interfaces.Services.Finance;
using Domain.Models.Finance;

namespace Domain.Services
{
    public class DespesasDomainService : IDespesasDomainService
    {
        public double CalculaTotalDespesaForaAlmocoAluguelCondominioContaDeLuz(
            List<Despesa> despesas,
            CategoriaIdsDto categoriaIds
        )
        {
            double total = despesas
                .Where(d =>
                    d.CategoriaId != categoriaIds.IdAluguel
                    && d.CategoriaId != categoriaIds.IdCondominio
                    && d.CategoriaId != categoriaIds.IdContaDeLuz
                    && d.CategoriaId != categoriaIds.IdAlmoco
                )
                .Sum(d => d.Total);

            return total;
        }

        public RelatorioGastosDoMesDto GetRelatorioDeGastosDoMes(
            string mesAtual,
            CategoriaIdsDto categIds,
            List<Despesa> despesas
        )
        {
            double aluguelMaisCondominio = despesas
                .Where(d =>
                    d.Categoria.Id == categIds.IdAluguel || d.Categoria.Id == categIds.IdCondominio
                )
                .Sum(d => d.Total);

            double totalGeral = despesas.Sum(d => d.Total);

            double totalGastosGerais = totalGeral - aluguelMaisCondominio;

            return new RelatorioGastosDoMesDto(
                mesAtual,
                aluguelMaisCondominio,
                totalGastosGerais,
                totalGeral
            );
        }

        public (double, double) CalculaTotalAlmocoDivididoComJhon(
            List<Despesa> despesas,
            int idAlmoco,
            int todosMembros
        )
        {
            double almoco = despesas.Where(d => d.CategoriaId == idAlmoco).Sum(d => d.Total);

            double almocoParteDoJhon = almoco / todosMembros;

            double almocoAbatido = almoco - almocoParteDoJhon;

            return (almocoAbatido, almocoParteDoJhon);
        }

        public (double, double) CalcularTotalAluguelCondominioContaDeLuzPorMembro(
            List<Despesa> despesas,
            CategoriaIdsDto categoriaIds,
            List<Membro> todosMembros,
            int idJhon,
            int idPeu
        )
        {
            List<Membro> membrosForaJhon = todosMembros
                .Where(membro => membro.Id != idJhon)
                .ToList();

            List<Membro> membrosForaPeuJhon = membrosForaJhon
                .Where(membro => membro.Id != idPeu)
                .ToList();

            double valorAluguel = despesas
                .Where(d => d.CategoriaId == categoriaIds.IdAluguel)
                .Sum(aluguel => aluguel.Total);

            double valorCondominio = despesas
                .Where(d => d.CategoriaId == categoriaIds.IdCondominio)
                .Sum(condominio => condominio.Total);

            double valorContaDeLuz = despesas
                .Where(d => d.CategoriaId == categoriaIds.IdContaDeLuz)
                .Sum(despesa => despesa.Total);

            double luzMaisCondominioPorMembro =
                (valorCondominio + valorContaDeLuz - 100) / membrosForaJhon.Count; //100 reais referente ao estacionamento que alugamos.

            double aluguelPorMembroForaPeu = (valorAluguel - 300) / membrosForaPeuJhon.Count; //300 reais é o valor do aluguel do peu.

            double aluguelCondominioContaLuzPorMembroForaPeu =
                aluguelPorMembroForaPeu + luzMaisCondominioPorMembro;

            double aluguelCondominioContaLuzParaPeu = 300 + luzMaisCondominioPorMembro;

            return (aluguelCondominioContaLuzPorMembroForaPeu, aluguelCondominioContaLuzParaPeu);
        }
    }
}
