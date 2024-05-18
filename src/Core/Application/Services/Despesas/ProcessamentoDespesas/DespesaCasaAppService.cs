using Application.Interfaces.Services.Despesas;
using Application.Services.Despesas.Base;
using Domain.Dtos.Despesas.Relatorios;
using Domain.Models.Membros;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Despesas.ProcessamentoDespesas
{
    public class DespesaCasaAppService(IServiceProvider service)
        : BaseDespesaService(service),
            IDespesaCasaAppService
    {
        public async Task<DistribuicaoCustosCasaDto> CalcularDistribuicaoCustosCasaAsync()
        {
            List<Membro> todosMembros = await _membroRepository.Get().ToListAsync();

            List<Membro> listMembersForaJhon = todosMembros
                .Where(m => m.Id != _membroId.IdJhon)
                .ToList();

            // Despesas gerais Limpesa, Higiêne etc... (Fora Almoço, e despesa Moradia aluguel, luz etc..) :
            double totalDespesaGerais = await CalculaTotalDespesaForaAlmocoDespesaMoradiaAsync();

            //  Almoço divido com Jhon
            var (totalAlmocoDividioComJhon, totalAlmocoParteDoJhon) =
                await CalculaTotalAlmocoDivididoComJhonAsync();

            //Despesa gerais Limpesa, Higiêne etc... somado com Almoço divido com Jhon
            double valorTotalDoAlmoco = totalAlmocoDividioComJhon + totalAlmocoParteDoJhon;
            double despesaGeraisMaisAlmoco = totalDespesaGerais + valorTotalDoAlmoco;

            double despesaGeraisMaisAlmocoDividioPorMembro =
                despesaGeraisMaisAlmoco / listMembersForaJhon.Count;

            return new DistribuicaoCustosCasaDto()
            {
                Membros = todosMembros,
                TotalDespesaGerais = totalDespesaGerais,
                TotalAlmocoDividioComJhon = totalAlmocoDividioComJhon,
                TotalAlmocoParteDoJhon = totalAlmocoParteDoJhon,
                DespesaGeraisMaisAlmoco = despesaGeraisMaisAlmoco,
                DespesaGeraisMaisAlmocoDividioPorMembro = despesaGeraisMaisAlmocoDividioPorMembro
            };
        }

        #region Metodos de Suporte
        private async Task<double> CalculaTotalDespesaForaAlmocoDespesaMoradiaAsync()
        {
            double total = await ListDespesasPorGrupo
                .Where(d =>
                    d.CategoriaId != _categoriaIds.IdAluguel
                    && d.CategoriaId != _categoriaIds.IdCondominio
                    && d.CategoriaId != _categoriaIds.IdContaDeLuz
                    && d.CategoriaId != _categoriaIds.IdAlmoco
                )
                .SumAsync(d => d.Total);

            return total;
        }

        private async Task<(double, double)> CalculaTotalAlmocoDivididoComJhonAsync()
        {
            int todosMembros = await _membroRepository.Get().CountAsync();

            double almoco = await ListDespesasPorGrupo
                .Where(despesa => despesa.CategoriaId == _categoriaIds.IdAlmoco)
                .SumAsync(despesa => despesa.Total);

            double almocoParteDoJhon = almoco / todosMembros;

            double almocoAbatido = almoco - almocoParteDoJhon;

            return (Math.Max(almocoAbatido, 0), Math.Max(almocoParteDoJhon, 0));
        }
        #endregion
    }
}
