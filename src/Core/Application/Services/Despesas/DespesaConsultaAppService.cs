using Application.Extensions.Help;
using Application.Interfaces.Services.Despesas;
using Application.Resources.Messages;
using Application.Services.Despesas.Base;
using Domain.Dtos.Despesas.Consultas;
using Domain.Dtos.Despesas.Relatorios;
using Domain.Dtos.Despesas.Resumos;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Membros;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Despesas
{
    public class DespesaConsultaAppService(
        IServiceProvider service,
        IDespesaCasaAppService _despesaCasaApp,
        IDespesaMoradiaAppService _despesaMoradiaApp,
        IGrupoDespesaRepository _grupoDespesaRepository
    ) : BaseDespesaService(service), IDespesaConsultaAppService
    {
        #region Consultas
        public IEnumerable<DespesasTotalPorCategoria> GetTotalPorCategoria()
        {
            if (ListDespesasPorGrupo.Count <= 0)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.DespesasNaoEncontradas, "")
                );
                return [];
            }

            var listAgrupada = ListDespesasPorGrupo.GroupBy(despesa => despesa.Categoria.Descricao);

            return listAgrupada.Select(list => new DespesasTotalPorCategoria(
                list.Key,
                list.Sum(despesa => despesa.Total)
            ));
        }

        public async Task<IEnumerable<DespesasPorGrupoDto>> GetDespesaGrupoParaGraficoAsync()
        {
            // Mapeamento de nomes de meses para valores numéricos
            var monthOrder = new Dictionary<string, int>
            {
                { "Janeiro", 1 },
                { "Fevereiro", 2 },
                { "Março", 3 },
                { "Abril", 4 },
                { "Maio", 5 },
                { "Junho", 6 },
                { "Julho", 7 },
                { "Agosto", 8 },
                { "Setembro", 9 },
                { "Outubro", 10 },
                { "Novembro", 11 },
                { "Dezembro", 12 }
            };

            var despesasPorGrupo = _repository
                .Get()
                .GroupBy(d => d.GrupoDespesa.Nome)
                .Select(group => new DespesasPorGrupoDto
                {
                    GrupoNome = group.Key,
                    Total = group.Sum(d => d.Total)
                })
                .AsEnumerable() // Processamento local para manipulação de strings e datas
                .OrderBy(dto =>
                {
                    // Extrai o nome do mês do GrupoNome
                    var monthName = dto.GrupoNome.Split(' ')[2]; // Assumindo "Fatura de Janeiro 2024"
                    // Usa o dicionário para encontrar a ordem do mês
                    return monthOrder[monthName];
                });

            return await Task.FromResult(despesasPorGrupo.ToList());
        }

        public async Task<DespesasDivididasMensalDto> GetAnaliseDesesasPorGrupoAsync()
        {
            //Aluguel + Condomínio + Conta de Luz
            var distribuicaoCustosMoradia =
                await _despesaMoradiaApp.CalcularDistribuicaoCustosMoradiaAsync();

            //Despesas de casa como almoço, Limpeza, higiene etc...
            var distribuicaoCustosCasa =
                await _despesaCasaApp.CalcularDistribuicaoCustosCasaAsync();

            var despesasPorMembro = await DistribuirDespesasEntreMembrosAsync(
                distribuicaoCustosCasa.DespesaGeraisMaisAlmocoDividioPorMembro,
                distribuicaoCustosCasa.TotalAlmocoParteDoJhon,
                distribuicaoCustosMoradia.DistribuicaoCustos.ValorParaMembrosForaPeu,
                distribuicaoCustosMoradia.DistribuicaoCustos.ValorParaDoPeu
            );

            var relatorioGastosDoGrupo = GetRelatorioDeGastosDoMes();

            return new DespesasDivididasMensalDto
            {
                RelatorioGastosDoGrupo = relatorioGastosDoGrupo,

                DespesasPorMembro = despesasPorMembro
            };
        }

        #endregion

        #region Metodos de Suporte
        private RelatorioGastosDoGrupoDto GetRelatorioDeGastosDoMes()
        {
            string grupoNome = _grupoDespesaRepository
                .Get(g => g.Id == _grupoDespesaId)
                .FirstOrDefault()
                ?.Nome;

            if (grupoNome.IsNullOrEmpty())
            {
                Notificar(EnumTipoNotificacao.Informacao, Message.SelecioneUmGrupoDesesa);
                return new();
            }

            double totalGastoMoradia = ListDespesasPorGrupo
                .Where(d =>
                    d.Categoria.Id == _categoriaIds.IdAluguel
                    || d.Categoria.Id == _categoriaIds.IdCondominio
                    || d.Categoria.Id == _categoriaIds.IdContaDeLuz
                )
                .Sum(d => d.Total);

            double totalGastosCasa = ListDespesasPorGrupo
                .Where(d =>
                    d.Categoria.Id != _categoriaIds.IdAluguel
                    && d.Categoria.Id != _categoriaIds.IdCondominio
                    && d.Categoria.Id != _categoriaIds.IdContaDeLuz
                )
                .Sum(d => d.Total);

            var totalGeral = totalGastoMoradia + totalGastosCasa;

            return new RelatorioGastosDoGrupoDto
            {
                GrupoDespesaNome = grupoNome,
                TotalGeral = totalGeral.RoundTo(2),
                TotalGastosCasa = totalGastosCasa.RoundTo(2),
                TotalGastosMoradia = totalGastoMoradia.RoundTo(2),
            };
        }

        private async Task<IEnumerable<DespesaPorMembroDto>> DistribuirDespesasEntreMembrosAsync(
            double despesaGeraisMaisAlmocoDividioPorMembro,
            double almocoParteDoJhon,
            double aluguelCondominioContaLuzPorMembroForaPeu,
            double aluguelCondominioContaLuzParaPeu
        )
        {
            var todosMembers = await _membroRepository.Get().ToListAsync();

            double ValorMoradia(Membro membro)
            {
                if (membro.Id == _membroId.IdPeu)
                {
                    return aluguelCondominioContaLuzParaPeu.RoundTo(2);
                }
                else
                {
                    return aluguelCondominioContaLuzPorMembroForaPeu.RoundTo(2);
                }
            }

            var valoresPorMembro = todosMembers.Select(member => new DespesaPorMembroDto
            {
                Nome = member.Nome,

                ValorDespesaCasa =
                    member.Id == _membroId.IdJhon
                        ? Math.Max(almocoParteDoJhon.RoundTo(2), 0)
                        : Math.Max(despesaGeraisMaisAlmocoDividioPorMembro.RoundTo(2), 0),

                ValorDespesaMoradia =
                    member.Id == _membroId.IdJhon ? -1 : ValorMoradia(member).RoundTo(2)
            });

            return valoresPorMembro;
        }
        #endregion
    }
}
