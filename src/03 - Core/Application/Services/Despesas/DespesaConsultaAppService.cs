using Application.Extensions.Help;
using Application.Interfaces.Services.Despesas;
using Application.Services.Base;
using Domain.Dtos.Despesas.Consultas;
using Domain.Dtos.Despesas.Relatorios;
using Domain.Dtos.Despesas.Resumos;
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
            var (inicioDoMes, fimDoMes) = await GetPeriodoParaCalculoAsync();

            var listDespesas = _repository
                .Get(d => d.DataCompra >= inicioDoMes && d.DataCompra <= fimDoMes)
                .Include(c => c.Categoria);

            var listAgrupada = listDespesas.GroupBy(despesa => despesa.Categoria.Descricao);

            return await listAgrupada
                .Select(list => new DespesasTotalPorCategoria(
                    list.Key,
                    list.Sum(despesa => despesa.Total)
                ))
                .ToListAsync();
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
            var distribuicaoCustosHabitacional =
                await CalcularDistribuicaoCustosHabitacionalAsync();

            var distribuicaoCustosCasa = await CalcularDistribuicaoCustosCasaAsync();

            return new ResumoMensalDto
            {
                RelatorioGastosDoMes = GetRelatorioDeGastosDoMes(),

                DespesasPorMembro = DistribuirDespesasEntreMembros(
                    listTodosMembros,
                    distribuicaoCustosCasa.DespesaGeraisMaisAlmocoDividioPorMembro,
                    distribuicaoCustosCasa.TotalAlmocoParteDoJhon,
                    distribuicaoCustosHabitacional.DistribuicaoCustos.ValorParaMembrosForaPeu,
                    distribuicaoCustosHabitacional.DistribuicaoCustos.ValorParaDoPeu
                )
            };
        }

        #endregion

        #region Support Methods

        public async Task<DetalhamentoDespesasHabitacionalDto> CalcularDistribuicaoCustosHabitacionalAsync()
        {
            var categoriaIds = _categoriaRepository.GetCategoriaIds();
            var (idJhon, idPeu) = _membroRepository.GetIdsJhonPeu();

            var listAluguel = await GetDespesasMaisRecentes(despesa =>
                    despesa.CategoriaId == categoriaIds.IdAluguel
                )
                .ToListAsync();

            List<Membro> listMembroForaJhon = await _membroRepository
                .Get(m => m.Id != idJhon)
                .ToListAsync();

            List<Membro> listMembroForaJhonPeu = listMembroForaJhon
                .Where(m => m.Id != idPeu)
                .ToList();

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

            double contaDeLuz =
                GetDespesasMaisRecentes(despesa => despesa.CategoriaId == categoriaIds.IdContaDeLuz)
                    .FirstOrDefault()
                    ?.Preco ?? 0;

            double condominio =
                GetDespesasMaisRecentes(despesa => despesa.CategoriaId == categoriaIds.IdCondominio)
                    .FirstOrDefault()
                    ?.Preco ?? 0;

            double totalAptoMaisCaixa = parcelaApartamento + parcelaCaixa;
            double totalLuzMaisCondominio = contaDeLuz + condominio;

            double totalAptoMaisCaixaAbate300Peu = totalAptoMaisCaixa - 300; //300 aluguel cobrado do peu
            double totalLuzMaisCondominioAbate100Estacionamento = totalLuzMaisCondominio - 100; //estacionamento alugado

            double valorAptoMaisCaixaParaCadaMembro =
                totalAptoMaisCaixaAbate300Peu / listMembroForaJhonPeu.Count;

            double valorLuzMaisCondominioParaCadaMembro =
                totalLuzMaisCondominioAbate100Estacionamento / listMembroForaJhon.Count;

            double valorParaMembrosForaPeu =
                valorAptoMaisCaixaParaCadaMembro + valorLuzMaisCondominioParaCadaMembro;

            double valorParaDoPeu = 300 + valorLuzMaisCondominioParaCadaMembro;

            var distribuicaoCustosHabitacional = new DetalhamentoDespesasHabitacionalDto()
            {
                IdPeu = idPeu,
                ListAluguel = listAluguel,
                ListMembroForaJhon = listMembroForaJhon,
                ListMembroForaJhonPeu = listMembroForaJhonPeu,

                Condominio = condominio,
                ContaDeLuz = contaDeLuz,
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

            return distribuicaoCustosHabitacional;
        }

        public async Task<DistribuicaoCustosCasaDto> CalcularDistribuicaoCustosCasaAsync()
        {
            var (idJhon, _) = _membroRepository.GetIdsJhonPeu();

            IQueryable<Despesa> listDespesasMaisRecentes = GetDespesasMaisRecentes();

            List<Membro> todosMembros = await _membroRepository.Get().ToListAsync();

            List<Membro> listMembersForaJhon = todosMembros.Where(m => m.Id != idJhon).ToList();

            // Despesas gerais Limpesa, Higiêne etc... (Fora Almoço, e despesa Habitacional aluguel, luz etc..) :
            double totalDespesaGerais = CalculaTotalDespesaForaAlmocoDespesaHabitacional(
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
            IQueryable<Despesa> listDespesasMaisRecentes = GetDespesasMaisRecentes();
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

            return new RelatorioGastosDoMesDto(
                mesAtual,
                aluguelMaisCondominio,
                totalGastosGerais,
                totalGeral
            );
        }

        private double CalculaTotalDespesaForaAlmocoDespesaHabitacional(
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

                ValorDespesaHabitacional =
                    member.Id == idJhon ? 0 : ValorCondominioAluguelContaDeLuz(member)
            });

            return valoresPorMembro;
        }

        private IQueryable<Despesa> GetDespesasMaisRecentes(
            Expression<Func<Despesa, bool>> filter = null
        )
        {
            var (inicioDoMes, fimDoMes) = GetPeriodoParaCalculoAsync().Result;

            IQueryable<Despesa> query = _repository
                .Get(d => d.DataCompra >= inicioDoMes && d.DataCompra <= fimDoMes)
                .Include(c => c.Categoria);

            if(filter != null)
            {
                query = query.Where(filter);
            }

            return query;
        }

        private async Task<(DateTime, DateTime)> GetPeriodoParaCalculoAsync()
        {
            var dataMaisRecente = await _repository
                .Get()
                .OrderByDescending(d => d.DataCompra)
                .Select(d => d.DataCompra)
                .FirstOrDefaultAsync();

            var inicioDoMes = new DateTime(dataMaisRecente.Year, dataMaisRecente.Month, 1);
            var fimDoMes = inicioDoMes.AddMonths(1).AddDays(-1);

            return (inicioDoMes, fimDoMes);
        }
        #endregion
    }

    public class Name { }
}
