﻿using Application.Interfaces.Services.Despesas;
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
            double totalDespesaGerais = CalculaTotalDespesaForaAlmocoDespesaMoradiaAsync();

            //  Almoço divido com Jhon
            var (totalAlmocoDividioComJhon, totalAlmocoParteDoJhon, totalAlmoco) =
                await CalculaTotalAlmocoDivididoComJhonAsync();

            double despesaGeraisMaisAlmoco = totalDespesaGerais + totalAlmoco;

            //Despesa gerais Limpesa, Higiêne etc... somado com Almoço divido com Jhon
            double despesaGeraisMaisAlmocoDividioPorMembro =
                (totalDespesaGerais + totalAlmocoDividioComJhon) / listMembersForaJhon.Count;

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
        private double CalculaTotalDespesaForaAlmocoDespesaMoradiaAsync()
        {
            double total = ListDespesasPorGrupo
                .Where(d =>
                    d.CategoriaId != _categoriaIds.IdAluguel
                    && d.CategoriaId != _categoriaIds.IdCondominio
                    && d.CategoriaId != _categoriaIds.IdContaDeLuz
                    && d.CategoriaId != _categoriaIds.IdAlmoco
                )
                .Sum(d => d.Total);

            return total;
        }

        private async Task<(double, double, double)> CalculaTotalAlmocoDivididoComJhonAsync()
        {
            int todosMembros = await _membroRepository.Get().CountAsync();

            double totalAlmoco = ListDespesasPorGrupo
                .Where(despesa => despesa.CategoriaId == _categoriaIds.IdAlmoco)
                .Sum(despesa => despesa.Total);

            double almocoParteDoJhon = totalAlmoco / todosMembros;

            double almocoAbatido = totalAlmoco - almocoParteDoJhon;

            return (
                Math.Max(almocoAbatido, 0),
                Math.Max(almocoParteDoJhon, 0),
                Math.Max(totalAlmoco, 0)
            );
        }
        #endregion
    }
}
