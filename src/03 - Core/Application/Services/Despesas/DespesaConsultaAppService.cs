using Application.Extensions.Help;
using Application.Interfaces.Services.Despesas;
using Application.Resources.Messages;
using Application.Services.Base;
using Domain.Dtos.Despesas.Consultas;
using Domain.Dtos.Despesas.Relatorios;
using Domain.Dtos.Despesas.Resumos;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Domain.Models.Membros;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;

namespace Application.Services.Despesas
{
    public class DespesaConsultaAppService(
        IServiceProvider service,
        IMembroRepository _membroRepository,
        ICategoriaRepository _categoriaRepository
    ) : BaseAppService<Despesa, IDespesaRepository>(service), IDespesaConsultaAppService
    {
        #region Consultas
        public async Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoriaAsync()
        {
            var listDespesas = await GetDespesasMaisRecentes().ToListAsync();

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

        public async Task<ResumoMensalDto> GetResumoDespesasMensalAsync()
        {
            List<Membro> listTodosMembros = await _membroRepository.Get().ToListAsync();

            //Aluguel + Condomínio + Conta de Luz
            var distribuicaoCustosMoradia = await CalcularDistribuicaoCustosMoradiaAsync();

            var distribuicaoCustosCasa = await CalcularDistribuicaoCustosCasaAsync();

            return new ResumoMensalDto
            {
                RelatorioGastosDoMes = GetRelatorioDeGastosDoMes(),

                DespesasPorMembro = DistribuirDespesasEntreMembros(
                    listTodosMembros,
                    distribuicaoCustosCasa.DespesaGeraisMaisAlmocoDividioPorMembro,
                    distribuicaoCustosCasa.TotalAlmocoParteDoJhon,
                    distribuicaoCustosMoradia.DistribuicaoCustos.ValorParaMembrosForaPeu,
                    distribuicaoCustosMoradia.DistribuicaoCustos.ValorParaDoPeu
                )
            };
        }

        public async Task<(double, double)> ConferirFaturaDoCartaoComDespesasAsync(
            double faturaCartao
        )
        {
            var listDespesas = await GetDespesasMaisRecentes().ToListAsync();

            double totalDespesas = listDespesas.Sum(despesa => despesa.Total);

            double valorSubtraido = totalDespesas - faturaCartao;

            return (totalDespesas, valorSubtraido);
        }

        #endregion

        #region Support Methods

        public async Task<DetalhamentoDespesasMoradiaDto> CalcularDistribuicaoCustosMoradiaAsync()
        {
            var categoriaIds = _categoriaRepository.GetCategoriaIds();
            var (idJhon, idPeu) = _membroRepository.GetIdsJhonPeu();

            var listAluguel = await GetDespesasMaisRecentes(d =>
                    d.CategoriaId == categoriaIds.IdAluguel
                )
                .ToListAsync();

            var contaDeLuz = await GetDespesasMaisRecentes(despesa =>
                    despesa.CategoriaId == categoriaIds.IdContaDeLuz
                )
                .FirstOrDefaultAsync();

            var condominio = await GetDespesasMaisRecentes(despesa =>
                    despesa.CategoriaId == categoriaIds.IdCondominio
                )
                .FirstOrDefaultAsync();

            if(listAluguel.Count <= 0 && condominio is null && contaDeLuz is null)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.DespesasNaoEncontradas, "de Moradia")
                );
                return new DetalhamentoDespesasMoradiaDto();
            }

            List<Membro> listMembroForaJhon = await _membroRepository
                .Get(m => m.Id != idJhon)
                .ToListAsync();

            List<Membro> listMembroForaJhonPeu = listMembroForaJhon
                .Where(m => m.Id != idPeu)
                .ToList();

            double contaDeLuzValue = contaDeLuz?.Preco ?? 0;

            double condominioValue = condominio?.Preco ?? 0;

            double parcelaApartamento =
                listAluguel
                    .Where(a =>
                        a.Item.Contains("ap ponto", StringComparison.CurrentCultureIgnoreCase)
                    )
                    .FirstOrDefault()
                    ?.Preco ?? 0;

            double parcelaCaixa =
                listAluguel
                    .Where(a => a.Item.Contains("caixa", StringComparison.CurrentCultureIgnoreCase))
                    .FirstOrDefault()
                    ?.Preco ?? 0;



            double totalAptoMaisCaixa = parcelaApartamento + parcelaCaixa;
            double totalLuzMaisCondominio = contaDeLuzValue + condominioValue;

            double totalAptoMaisCaixaAbate300Peu = totalAptoMaisCaixa - 300; //300 aluguel cobrado do peu
            double totalLuzMaisCondominioAbate100Estacionamento = totalLuzMaisCondominio - 100; //estacionamento alugado

            double valorAptoMaisCaixaParaCadaMembro =
                totalAptoMaisCaixaAbate300Peu / listMembroForaJhonPeu.Count;

            double valorLuzMaisCondominioParaCadaMembro =
                totalLuzMaisCondominioAbate100Estacionamento / listMembroForaJhon.Count;

            double valorParaMembrosForaPeu =
                valorAptoMaisCaixaParaCadaMembro + valorLuzMaisCondominioParaCadaMembro;

            double valorParaDoPeu = 300 + valorLuzMaisCondominioParaCadaMembro;

            var distribuicaoCustosMoradia = new DetalhamentoDespesasMoradiaDto()
            {
                IdPeu = idPeu,
                ListAluguel = listAluguel,
                ListMembroForaJhon = listMembroForaJhon,
                ListMembroForaJhonPeu = listMembroForaJhonPeu,

                Condominio = condominioValue,
                ContaDeLuz = contaDeLuzValue,
                ParcelaCaixa = parcelaCaixa,
                ParcelaApartamento = parcelaApartamento,

                DistribuicaoCustos = new()
                {
                    ValorParaDoPeu = valorParaDoPeu,
                    TotalAptoMaisCaixa = totalAptoMaisCaixa,
                    TotalLuzMaisCondominio = totalLuzMaisCondominio,
                    ValorParaMembrosForaPeu = valorParaMembrosForaPeu,
                    TotalAptoMaisCaixaAbate300Peu = totalAptoMaisCaixaAbate300Peu,
                    ValorAptoMaisCaixaParaCadaMembro = valorAptoMaisCaixaParaCadaMembro,
                    ValorLuzMaisCondominioParaCadaMembro = valorLuzMaisCondominioParaCadaMembro,
                    TotalLuzMaisCondominioAbate100Estacionamento =
                        totalLuzMaisCondominioAbate100Estacionamento,
                }
            };

            return distribuicaoCustosMoradia;
        }

        public async Task<DistribuicaoCustosCasaDto> CalcularDistribuicaoCustosCasaAsync()
        {
            var (idJhon, _) = _membroRepository.GetIdsJhonPeu();

            IQueryable<Despesa> listDespesasMaisRecentes = GetDespesasMaisRecentes();

            List<Membro> todosMembros = await _membroRepository.Get().ToListAsync();

            List<Membro> listMembersForaJhon = todosMembros.Where(m => m.Id != idJhon).ToList();

            // Despesas gerais Limpesa, Higiêne etc... (Fora Almoço, e despesa Moradia aluguel, luz etc..) :
            double totalDespesaGerais = CalculaTotalDespesaForaAlmocoDespesaMoradia(
                listDespesasMaisRecentes
            );

            //  Almoço divido com Jhon
            var (totalAlmocoDividioComJhon, totalAlmocoParteDoJhon) =
                await CalculaTotalAlmocoDivididoComJhon(listDespesasMaisRecentes);

            //Despesa gerais Limpesa, Higiêne etc... somado com Almoço divido com Jhon
            double despesaGeraisMaisAlmoco = totalDespesaGerais + totalAlmocoDividioComJhon;

            double despesaGeraisMaisAlmocoDividioPorMembro =
                despesaGeraisMaisAlmoco / listMembersForaJhon.Count;

            return new DistribuicaoCustosCasaDto()
            {
                IdJhon = idJhon,
                Membros = todosMembros,
                TotalDespesaGerais = totalDespesaGerais,
                TotalAlmocoDividioComJhon = totalAlmocoDividioComJhon,
                TotalAlmocoParteDoJhon = totalAlmocoParteDoJhon,
                DespesaGeraisMaisAlmoco = despesaGeraisMaisAlmoco,
                DespesaGeraisMaisAlmocoDividioPorMembro = despesaGeraisMaisAlmocoDividioPorMembro
            };
        }

        private async Task<(double, double)> CalculaTotalAlmocoDivididoComJhon(
            IQueryable<Despesa> listDespesasMaisRecentes
        )
        {
            var categoriaIds = _categoriaRepository.GetCategoriaIds();
            var todosMembros = await _membroRepository.Get().CountAsync();

            double almoco = listDespesasMaisRecentes
                .Where(d => d.CategoriaId == categoriaIds.IdAlmoco)
                .Sum(d => d.Total);

            double almocoParteDoJhon = almoco / todosMembros;

            double almocoAbatido = almoco - almocoParteDoJhon;

            return (almocoAbatido, almocoParteDoJhon);
        }

        private RelatorioGastosDoMesDto GetRelatorioDeGastosDoMes()
        {
            List<Despesa> listDespesasMaisRecentes = GetDespesasMaisRecentes().ToList();

            if(listDespesasMaisRecentes.Count <= 0)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    string.Format(Message.DespesasNaoEncontradas, "")
                );
                return new RelatorioGastosDoMesDto();
            }

            string mesAtual = listDespesasMaisRecentes
                .FirstOrDefault()
                .DataCompra.ToString("Y", new CultureInfo("pt-BR"));

            var categIds = _categoriaRepository.GetCategoriaIds();

            double aluguelMaisCondominio = listDespesasMaisRecentes
                .Where(d =>
                    d.Categoria.Id == categIds.IdAluguel || d.Categoria.Id == categIds.IdCondominio
                )
                .Sum(d => d.Total);

            double totalGeral = listDespesasMaisRecentes.Sum(d => d.Total);

            double totalGastosGerais = totalGeral - aluguelMaisCondominio;

            return new RelatorioGastosDoMesDto
            {
                MesAtual = mesAtual,
                TotalGeral = totalGeral,
                TotalGastosCasa = totalGastosGerais,
                TotalGastosMoradia = totalGastosGerais,
            };
        }

        private double CalculaTotalDespesaForaAlmocoDespesaMoradia(
            IQueryable<Despesa> listDespesasMaisRecentes
        )
        {
            var categoriaIds = _categoriaRepository.GetCategoriaIds();

            double total = listDespesasMaisRecentes
                .Where(d =>
                    d.CategoriaId != categoriaIds.IdAluguel
                    && d.CategoriaId != categoriaIds.IdCondominio
                    && d.CategoriaId != categoriaIds.IdContaDeLuz
                    && d.CategoriaId != categoriaIds.IdAlmoco
                )
                .Sum(d => d.Total);

            return total;
        }

        private IEnumerable<DespesaPorMembroDto> DistribuirDespesasEntreMembros(
            List<Membro> members,
            double despesaGeraisMaisAlmocoDividioPorMembro,
            double totalAlmocoDividioComJhon,
            double aluguelCondominioContaLuzPorMembroForaPeu,
            double aluguelCondominioContaLuzParaPeu
        )
        {
            var (idJhon, idPeu) = _membroRepository.GetIdsJhonPeu();

            double ValorCondominioAluguelContaDeLuz(Membro membro)
            {
                if(membro.Id == idPeu)
                {
                    return aluguelCondominioContaLuzParaPeu.RoundTo(2);
                }
                else
                {
                    return aluguelCondominioContaLuzPorMembroForaPeu.RoundTo(2);
                }
            }

            var valoresPorMembro = members.Select(member => new DespesaPorMembroDto
            {
                Nome = member.Nome,

                ValorDespesaCasa =
                    member.Id == idJhon
                        ? totalAlmocoDividioComJhon.RoundTo(2)
                        : despesaGeraisMaisAlmocoDividioPorMembro.RoundTo(2),

                ValorDespesaMoradia =
                    member.Id == idJhon ? -1 : ValorCondominioAluguelContaDeLuz(member).RoundTo(2)
            });

            return valoresPorMembro;
        }

        private IQueryable<Despesa> GetDespesasMaisRecentes(
            Expression<Func<Despesa, bool>> filter = null
        )
        {
            var dataMaisRecente = _repository
                .Get()
                .OrderByDescending(d => d.DataCompra)
                .Select(d => d.DataCompra)
                .FirstOrDefault();

            var inicioDoMes = new DateTime(dataMaisRecente.Year, dataMaisRecente.Month, 1);
            var fimDoMes = inicioDoMes.AddMonths(1).AddDays(-1);

            IQueryable<Despesa> query = _repository
                .Get(d => d.DataCompra.Date >= inicioDoMes && d.DataCompra.Date <= fimDoMes)
                .Include(c => c.Categoria);

            var t = _repository.Get().Select(d => d.DataCompra).ToList();

            if(filter != null)
            {
                query = query.Where(filter);
            }

            return query;
        }
        #endregion
    }
}
