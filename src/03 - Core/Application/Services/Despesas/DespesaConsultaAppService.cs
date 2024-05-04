using Application.Extensions.Help;
using Application.Interfaces.Services.Despesas;
using Application.Resources.Messages;
using Application.Services.Despesas.Base;
using Domain.Dtos.Despesas.Consultas;
using Domain.Dtos.Despesas.Relatorios;
using Domain.Dtos.Despesas.Resumos;
using Domain.Enumeradores;
using Domain.Models.Membros;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Application.Services.Despesas
{
    public class DespesaConsultaAppService(
        IServiceProvider service,
        IDespesaCasaAppService _despesaCasaApp,
        IDespesaMoradiaAppService _despesaMoradiaApp
    ) : BaseDespesaService(service), IDespesaConsultaAppService
    {
        #region Consultas
        public async Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoriaAsync()
        {
            var listDespesas = await ListDespesasRecentes.ToListAsync();

            if(listDespesas.Count <= 0)
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

        public async Task<IEnumerable<DespesasPorMesDto>> GetTotaisComprasPorMesAsync()
        {
            var despesasPorMes = _repository
                .Get()
                .GroupBy(d => new { d.DataCompra.Year, d.DataCompra.Month })
                .OrderBy(g => g.Key.Month)
                .ThenBy(g => g.Key.Month)
                .Select(group => new DespesasPorMesDto(
                    new DateTime(group.Key.Year, group.Key.Month, 1).ToString(
                        "MMMM",
                        new CultureInfo("pt-BR")
                    ),
                    group.Sum(d => d.Total).RoundTo(2)
                ));

            return await despesasPorMes.ToListAsync();
        }

        public async Task<DespesasDivididasMensalDto> GetDespesasDivididasMensalAsync()
        {
            //Aluguel + Condomínio + Conta de Luz
            var distribuicaoCustosMoradia =
                await _despesaMoradiaApp.CalcularDistribuicaoCustosMoradiaAsync();

            //Despesas de casa como almoço, Limpeza, higiene etc...
            var distribuicaoCustosCasa =
                await _despesaCasaApp.CalcularDistribuicaoCustosCasaAsync();

            return new DespesasDivididasMensalDto
            {
                RelatorioGastosDoMes = GetRelatorioDeGastosDoMesAsync(),

                DespesasPorMembro = await DistribuirDespesasEntreMembros(
                    distribuicaoCustosCasa.DespesaGeraisMaisAlmocoDividioPorMembro,
                    distribuicaoCustosCasa.TotalAlmocoParteDoJhon,
                    distribuicaoCustosMoradia.DistribuicaoCustos.ValorParaMembrosForaPeu,
                    distribuicaoCustosMoradia.DistribuicaoCustos.ValorParaDoPeu
                )
            };
        }

        #endregion

        #region Metodos de Suporte
        private RelatorioGastosDoMesDto GetRelatorioDeGastosDoMesAsync()
        {
            var despesaRecente = ListDespesasRecentes.FirstOrDefault();

            if(despesaRecente is null)
            {
                return new();
            }

            string mesAtual = despesaRecente.DataCompra.ToString("Y", new CultureInfo("pt-BR"));

            double aluguelMaisCondominio = ListDespesasRecentes
                .Where(d =>
                    d.Categoria.Id == _categoriaIds.IdAluguel
                    || d.Categoria.Id == _categoriaIds.IdCondominio
                )
                .Sum(d => d.Total);

            double totalGeral = ListDespesasRecentes.Sum(d => d.Total);

            double totalGastosGerais = totalGeral - aluguelMaisCondominio;

            return new RelatorioGastosDoMesDto
            {
                MesAtual = mesAtual,
                TotalGeral = totalGeral,
                TotalGastosCasa = totalGastosGerais,
                TotalGastosMoradia = totalGastosGerais,
            };
        }

        private async Task<IEnumerable<DespesaPorMembroDto>> DistribuirDespesasEntreMembros(
            double despesaGeraisMaisAlmocoDividioPorMembro,
            double almocoParteDoJhon,
            double aluguelCondominioContaLuzPorMembroForaPeu,
            double aluguelCondominioContaLuzParaPeu
        )
        {
            var todosMembers = await _membroRepository.Get().ToListAsync();

            double ValorMoradia(Membro membro)
            {
                if(membro.Id == _membroId.IdPeu)
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
