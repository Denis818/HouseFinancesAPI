using Application.Interfaces.Services.Despesas;
using Application.Services.Despesas.Base;
using Domain.Dtos.Despesas.Relatorios;
using Domain.Interfaces.Services.Despesa;
using Domain.Models.Membros;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Despesas.ProcessamentoDespesas
{
    public class DespesaCasaAppService(
        IServiceProvider service,
        IDespesaDomainServices _despesaDomainServices
    ) : BaseDespesaService(service), IDespesaCasaAppService
    {
        public async Task<DistribuicaoCustosCasaDto> CalcularDistribuicaoCustosCasaAsync()
        {
            List<Membro> todosMembros = await _membroRepository.Get().ToListAsync();

            int membrosForaJhonCount = todosMembros.Where(m => m.Id != _membroId.IdJhon).Count();

            // Despesas gerais Limpesa, Higiêne etc... (Fora Almoço)
            double totalDespesaGeraisForaAlmoco = await ListDespesasPorGrupo
                .Where(d =>
                    d.CategoriaId != _categoriaIds.IdAluguel
                    && d.CategoriaId != _categoriaIds.IdCondominio
                    && d.CategoriaId != _categoriaIds.IdContaDeLuz
                    && d.CategoriaId != _categoriaIds.IdAlmoco
                )
                .SumAsync(d => d.Total);

            //Total somente do almoço
            double valorTotalAlmoco = await ListDespesasPorGrupo
                .Where(despesa => despesa.CategoriaId == _categoriaIds.IdAlmoco)
                .SumAsync(despesa => despesa.Total);

            var custosDespesasCasa = new CustosDespesasCasaDto
            {
                TodosMembros = todosMembros,
                ValorTotalAlmoco = valorTotalAlmoco,
                TotalDespesaGeraisForaAlmoco = totalDespesaGeraisForaAlmoco,
                MembrosForaJhonCount = membrosForaJhonCount
            };

            var distribuicaoCustosCasa = _despesaDomainServices.CalcularDistribuicaoCustosCasa(
                custosDespesasCasa
            );

            return distribuicaoCustosCasa;
        }
    }
}
