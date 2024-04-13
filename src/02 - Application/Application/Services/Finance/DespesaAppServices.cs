using System.Globalization;
using Application.Configurations.Extensions.Help;
using Application.Constants;
using Application.Interfaces.Services.Finance;
using Application.Services.Base;
using Domain.Dtos.Finance;
using Domain.Enumeradores;
using Domain.Interfaces;
using Domain.Models.Finance;
using HouseFinancesAPI.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Finance
{
    public class DespesaAppServices(
        IServiceProvider service,
        IMembroRepository _membroRepository,
        ICategoriaRepository _categoriaRepository
    ) : BaseAppService<Despesa, IDespesaRepository>(service), IDespesaAppServices
    {
        #region CRUD
        public async Task<Despesa> GetByIdAsync(int id)
        {
            return await _repository
                .Get(despesa => despesa.Id == id)
                .Include(x => x.Categoria)
                .FirstOrDefaultAsync();
        }

        public async Task<PagedResult<Despesa>> GetAllAsync(int paginaAtual, int itensPorPagina)
        {
            var query = _repository.Get().Include(c => c.Categoria);
            return await Pagination.PaginateResultAsync(query, paginaAtual, itensPorPagina);
        }

        public async Task<Despesa> InsertAsync(DespesaDto despesaDto)
        {
            if (Validator(despesaDto))
                return null;

            if (await _categoriaRepository.ExisteAsync(despesaDto.CategoriaId) is null)
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    $"Categoria com id:{despesaDto.CategoriaId} não existe."
                );
                return null;
            }

            var despesa = _mapper.Map<Despesa>(despesaDto);

            despesa.Total = (despesa.Preco * despesa.Quantidade).RoundTo(2);

            await _repository.InsertAsync(despesa);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.InsertError);
                return null;
            }

            return await GetByIdAsync(despesa.Id);
        }

        public async Task<IEnumerable<Despesa>> InsertRangeAsync(
            IAsyncEnumerable<DespesaDto> listDespesasDto
        )
        {
            int totalRecebido = 0;
            var despesasParaInserir = new List<Despesa>();

            await foreach (var despesaDto in listDespesasDto)
            {
                totalRecebido++;

                if (Validator(despesaDto))
                    continue;

                if (await _categoriaRepository.ExisteAsync(despesaDto.CategoriaId) is null)
                {
                    Notificar(
                        EnumTipoNotificacao.Informacao,
                        $"Categoria com id:{despesaDto.CategoriaId} não existe."
                    );
                    continue;
                }

                var despesa = _mapper.Map<Despesa>(despesaDto);
                despesa.Total = (despesa.Preco * despesa.Quantidade).RoundTo(2);
                despesasParaInserir.Add(despesa);
            }

            if (despesasParaInserir.Count == 0)
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    "Nunhuma das despesa é valida para inserir."
                );
                return null;
            }

            await _repository.InsertRangeAsync(despesasParaInserir);
            if (!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.InsertError);
                return null;
            }

            if (totalRecebido > despesasParaInserir.Count)
            {
                Notificar(
                    EnumTipoNotificacao.Informacao,
                    $"{despesasParaInserir.Count} de {totalRecebido} despesas foram inseridas. "
                        + $"total de {totalRecebido - despesasParaInserir.Count} invalidas."
                );
            }

            var ids = despesasParaInserir.Select(d => d.Id).ToList();
            var despesasInseridas = await _repository
                .Get(d => ids.Contains(d.Id))
                .Include(c => c.Categoria)
                .ToListAsync();

            return despesasInseridas;
        }

        public async Task<Despesa> UpdateAsync(int id, DespesaDto despesaDto)
        {
            if (Validator(despesaDto))
                return null;

            var despesa = await _repository.GetByIdAsync(id);

            if (despesa == null)
            {
                Notificar(EnumTipoNotificacao.ClientError, ErrorMessages.NotFoundById + id);
                return null;
            }

            _mapper.Map(despesaDto, despesa);

            despesa.Total = despesa.Preco * despesa.Quantidade;

            _repository.Update(despesa);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.UpdateError);
                return null;
            }

            return await GetByIdAsync(despesa.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var despesa = await _repository.GetByIdAsync(id);

            if (despesa == null)
            {
                Notificar(EnumTipoNotificacao.ClientError, ErrorMessages.NotFoundById + id);
                return;
            }

            _repository.Delete(despesa);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.DeleteError);
                return;
            }

            Notificar(EnumTipoNotificacao.Informacao, "Registro Deletado");
        }
        #endregion

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
                .GroupBy(d => new { d.DataCompra.Month, d.DataCompra.Year })
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
            var (idAlmoco, idAluguel, idCondominio, idContaDeLuz) =
                _categoriaRepository.GetIdsAluguelAlmoco();

            var (inicioDoMes, fimDoMes) = await GetPeriodoParaCalculoAsync();

            List<Membro> listMembersForaJhon = await _membroRepository
                .Get(m => m.Nome != "Jhon Lenon")
                .ToListAsync();

            string mesAtual = inicioDoMes.ToString("Y", new CultureInfo("pt-BR"));

            List<Despesa> despesasAtuais = await _repository
                .Get(d => d.DataCompra >= inicioDoMes && d.DataCompra <= fimDoMes)
                .Include(c => c.Categoria)
                .ToListAsync();

            //Despesas Gerais Limpesa, Higiêne etc...
            decimal totalDespesaGerais = CalculaTotalDespesaForaAlmocoAluguelCondominioContaDeLuz(
                despesasAtuais,
                idAlmoco,
                idAluguel,
                idCondominio,
                idContaDeLuz
            );

            //Aluguel + Condomínio + Conta de Luz
            var (aluguelCondominioContaLuzPorMembroForaPeu, aluguelCondominioContaLuzParaPeu) =
                await CalcularTotalAluguelCondominioContaDeLuzPorMembro(
                    despesasAtuais,
                    idAluguel,
                    idCondominio,
                    idContaDeLuz
                );

            //Almoço divido com Jhon
            var (almocoDividioComJhon, almocoParteDoJhon) = CalculaTotalAlmocoDivididoComJhon(
                despesasAtuais,
                idAlmoco
            );

            //Despesa total da casa FORA aluguel e Conta de luz
            decimal totalDespesaGeraisMaisAlmocoDividio = totalDespesaGerais + almocoDividioComJhon;

            decimal totalDespesaGeraisMaisAlmocoPorMembro =
                totalDespesaGeraisMaisAlmocoDividio / listMembersForaJhon.Count;

            return new ResumoMensalDto
            {
                RelatorioGastosDoMes = GetRelatorioDeGastosDoMes(mesAtual, despesasAtuais),
                DespesasPorMembros = DistribuirDespesasEntreMembros(
                    totalDespesaGeraisMaisAlmocoPorMembro,
                    aluguelCondominioContaLuzPorMembroForaPeu,
                    aluguelCondominioContaLuzParaPeu,
                    almocoParteDoJhon
                )
            };
        }

        #region Support Methods

        public decimal CalculaTotalDespesaForaAlmocoAluguelCondominioContaDeLuz(
            List<Despesa> despesas,
            int idAlmoco,
            int idAluguel,
            int idCondominio,
            int idContaDeLuz
        )
        {
            decimal total = despesas
                .Where(d =>
                    d.CategoriaId != idAlmoco
                    && d.CategoriaId != idAluguel
                    && d.CategoriaId != idContaDeLuz
                    && d.CategoriaId != idCondominio
                )
                .Sum(d => d.Total);

            return total.RoundTo(2);
        }

        public RelatorioGastosDoMesDto GetRelatorioDeGastosDoMes(
            string mesAtual,
            List<Despesa> despesas
        )
        {
            decimal aluguelMaisCondominio = despesas
                .Where(d =>
                    d.Categoria.Descricao == "Aluguel" || d.Categoria.Descricao == "Condomínio"
                )
                .Sum(d => d.Total);

            decimal totalGeral = despesas.Sum(d => d.Total);

            decimal totalGastosGerais = totalGeral - aluguelMaisCondominio;

            return new RelatorioGastosDoMesDto(
                mesAtual,
                aluguelMaisCondominio,
                totalGastosGerais,
                totalGeral
            );
        }

        public (decimal, decimal) CalculaTotalAlmocoDivididoComJhon(
            List<Despesa> despesas,
            int idAlmoco
        )
        {
            decimal almoco = despesas.Where(d => d.CategoriaId == idAlmoco).Sum(d => d.Total);

            decimal almocoParteDoJhon = almoco / 5;

            decimal almocoAbatido = almoco - almocoParteDoJhon;

            return (almocoAbatido, almocoParteDoJhon);
        }

        private async Task<(decimal, decimal)> CalcularTotalAluguelCondominioContaDeLuzPorMembro(
            List<Despesa> despesas,
            int idAluguel,
            int idCondominio,
            int idContaDeLuz
        )
        {
            var (idJhon, idPeu) = _membroRepository.GetIdsJhonPeu();

            List<Membro> todosMembrosDaCasa = await _membroRepository
                .Get(membro => membro.Id != idJhon)
                .ToListAsync();

            List<Membro> membrosMenosPeu = todosMembrosDaCasa
                .Where(membro => membro.Id != idPeu)
                .ToList();

            decimal valorAluguel = despesas
                .Where(d => d.CategoriaId == idAluguel)
                .Sum(aluguel => aluguel.Total);

            decimal valorCondominio = despesas
                .Where(d => d.CategoriaId == idCondominio)
                .Sum(condominio => condominio.Total);

            decimal valorContaDeLuz = despesas
                .Where(d => d.CategoriaId == idContaDeLuz)
                .Sum(despesa => despesa.Total);

            decimal luzMaisCondominioPorMembro =
                (valorCondominio + valorContaDeLuz - 100) / todosMembrosDaCasa.Count; //100 reais referente ao estacionamento que alugamos.

            decimal aluguelPorMembroForaPeu = (valorAluguel - 300) / membrosMenosPeu.Count; //300 reais é o valor do aluguel do peu.

            decimal aluguelCondominioContaLuzPorMembroForaPeu =
                aluguelPorMembroForaPeu + luzMaisCondominioPorMembro;

            decimal aluguelCondominioContaLuzParaPeu = 300 + luzMaisCondominioPorMembro;

            return (aluguelCondominioContaLuzPorMembroForaPeu, aluguelCondominioContaLuzParaPeu);
        }

        private IEnumerable<DespesaPorMembroDto> DistribuirDespesasEntreMembros(
            decimal totalDespesaGeraisMaisAlmocoPorMembro,
            decimal aluguelCondominioContaLuzPorMembroForaPeu,
            decimal aluguelCondominioContaLuzParaPeu,
            decimal almocoDividioComJhon
        )
        {
            var members = _membroRepository.Get().ToList();
            var (idJhon, idPeu) = _membroRepository.GetIdsJhonPeu();

            return members.Select(member => new DespesaPorMembroDto
            {
                Nome = member.Nome,

                ValorDespesasCasa =
                    member.Id == idJhon
                        ? almocoDividioComJhon.RoundTo(2)
                        : totalDespesaGeraisMaisAlmocoPorMembro.RoundTo(2),

                ValorCondominioAluguelContaDeLuz =
                    member.Id == idPeu
                        ? aluguelCondominioContaLuzParaPeu.RoundTo(2)
                        : aluguelCondominioContaLuzPorMembroForaPeu.RoundTo(2)
            });
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
}
