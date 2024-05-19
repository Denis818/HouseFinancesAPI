using Application.Extensions.Help;
using Application.Interfaces.Services.Despesas;
using Application.Resources.Messages;
using Application.Services.Despesas.Base;
using Application.Services.Despesas.RelatorioPdf;
using Domain.Dtos.Despesas.Consultas;
using Domain.Dtos.Despesas.Relatorios;
using Domain.Dtos.Despesas.Resumos;
using Domain.Enumeradores;
using Domain.Models.Membros;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Despesas.Operacoes
{
    public class DespesaAppService(
        IDespesaCasaAppService _despesaCasaApp,
        IDespesaMoradiaAppService _despesaMoradiaApp,
        IServiceProvider service
    ) : BaseDespesaService(service), IDespesaAppService
    {
        #region Consultas
        public async Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoriaAsync()
        {
            var listDespesas = await ListDespesasPorGrupo.ToListAsync();

            if (listDespesas.Count <= 0)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.DespesasNaoEncontradas, "")
                );
                return [];
            }

            var listAgrupada = listDespesas.GroupBy(despesa => despesa.Categoria.Descricao);

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

            var relatorioGastosDoGrupo = await GetRelatorioDeGastosDoMesAsync();

            return new DespesasDivididasMensalDto
            {
                RelatorioGastosDoGrupo = relatorioGastosDoGrupo,

                DespesasPorMembro = despesasPorMembro
            };
        }
        #endregion

        #region Downloads
        public async Task<byte[]> DownloadPdfRelatorioDeDespesaCasa()
        {
            var custosCasaDto = await _despesaCasaApp.CalcularDistribuicaoCustosCasaAsync();

            return new DespesaCasaPdfReport().GerarRelatorioDespesaCasaPdf(custosCasaDto);
        }

        public async Task<byte[]> DownloadPdfRelatorioDeDespesaMoradia()
        {
            var custosMoradiaDto =
                await _despesaMoradiaApp.CalcularDistribuicaoCustosMoradiaAsync();

            return new DespesaMoradiaPdfReport().GerarRelatorioDespesaMoradiaPdf(custosMoradiaDto);
        }
        #endregion

        #region Utilitários
        public async Task<(double, double)> CompararFaturaComTotalDeDespesas(double faturaCartao)
        {
            double totalDespesas = await ListDespesasPorGrupo
                .Where(despesa =>
                    despesa.CategoriaId != _categoriaIds.IdAluguel
                    && despesa.CategoriaId != _categoriaIds.IdContaDeLuz
                    && despesa.CategoriaId != _categoriaIds.IdCondominio
                )
                .SumAsync(despesa => despesa.Total);

            double valorSubtraido = totalDespesas - faturaCartao;

            return (totalDespesas, valorSubtraido);
        }
        #endregion

        #region Metodos de Suporte
        private async Task<RelatorioGastosDoGrupoDto> GetRelatorioDeGastosDoMesAsync()
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

            double totalGastoMoradia = await ListDespesasPorGrupo
                .Where(d =>
                    d.Categoria.Id == _categoriaIds.IdAluguel
                    || d.Categoria.Id == _categoriaIds.IdCondominio
                    || d.Categoria.Id == _categoriaIds.IdContaDeLuz
                )
                .SumAsync(d => d.Total);

            double totalGastosCasa = await ListDespesasPorGrupo
                .Where(d =>
                    d.Categoria.Id != _categoriaIds.IdAluguel
                    && d.Categoria.Id != _categoriaIds.IdCondominio
                    && d.Categoria.Id != _categoriaIds.IdContaDeLuz
                )
                .SumAsync(d => d.Total);

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
            var todosMembros = await _membroRepository.Get().ToListAsync();

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

            var valoresPorMembro = todosMembros.Select(member => new DespesaPorMembroDto
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
