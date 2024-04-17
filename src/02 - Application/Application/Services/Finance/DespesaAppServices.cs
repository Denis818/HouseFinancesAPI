using Application.Constants;
using Application.Extensions.Help;
using Application.Interfaces.Services.Finance;
using Application.Services.Base;
using Domain.Dtos.Categoria;
using Domain.Dtos.Finance;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Finance;
using HouseFinancesAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
            if(Validator(despesaDto))
                return null;

            if(await _categoriaRepository.ExisteAsync(despesaDto.CategoriaId) is null)
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

            if(!await _repository.SaveChangesAsync())
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

            await foreach(var despesaDto in listDespesasDto)
            {
                totalRecebido++;

                if(Validator(despesaDto))
                    continue;

                if(await _categoriaRepository.ExisteAsync(despesaDto.CategoriaId) is null)
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

            if(despesasParaInserir.Count == 0)
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    "Nunhuma das despesa é valida para inserir."
                );
                return null;
            }

            await _repository.InsertRangeAsync(despesasParaInserir);
            if(!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.InsertError);
                return null;
            }

            if(totalRecebido > despesasParaInserir.Count)
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
            if(Validator(despesaDto))
                return null;

            var despesa = await _repository.GetByIdAsync(id);

            if(despesa == null)
            {
                Notificar(EnumTipoNotificacao.ClientError, ErrorMessages.NotFoundById + id);
                return null;
            }

            _mapper.Map(despesaDto, despesa);

            despesa.Total = despesa.Preco * despesa.Quantidade;

            _repository.Update(despesa);

            if(!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.UpdateError);
                return null;
            }

            return await GetByIdAsync(despesa.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var despesa = await _repository.GetByIdAsync(id);

            if(despesa == null)
            {
                Notificar(EnumTipoNotificacao.ClientError, ErrorMessages.NotFoundById + id);
                return;
            }

            _repository.Delete(despesa);

            if(!await _repository.SaveChangesAsync())
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
            var categoriaIds = _categoriaRepository.GetCategoriaIds();

            var (inicioDoMes, fimDoMes) = await GetPeriodoParaCalculoAsync();

            List<Membro> listMembersForaJhon = await _membroRepository
                .Get(m => m.Nome != "Jhon Lenon")
                .ToListAsync();

            string mesAtual = inicioDoMes.ToString("Y", new CultureInfo("pt-BR"));

            List<Despesa> despesasAtuais = await _repository
                .Get(d => d.DataCompra >= inicioDoMes && d.DataCompra <= fimDoMes)
                .Include(c => c.Categoria)
                .ToListAsync();

            //Despesas gerais Limpesa, Higiêne etc...
            double totalDespesaGerais = CalculaTotalDespesaForaAlmocoAluguelCondominioContaDeLuz(
                despesasAtuais,
                categoriaIds
            );

            //Aluguel + Condomínio + Conta de Luz
            var (aluguelCondominioContaLuzPorMembroForaPeu, aluguelCondominioContaLuzParaPeu) =
                await CalcularTotalAluguelCondominioContaDeLuzPorMembro(
                    despesasAtuais,
                    categoriaIds
                );

            //Almoço divido com Jhon
            var (totalAlmocoDividioComJhon, totalAlmocoParteDoJhon) =
                CalculaTotalAlmocoDivididoComJhon(despesasAtuais, categoriaIds.IdAlmoco);

            //Despesa gerais Limpesa, Higiêne etc... + Almoço divido com Jhon
            double despesaGeraisMaisAlmocoDividioPorMembro =
                (totalDespesaGerais + totalAlmocoDividioComJhon) / listMembersForaJhon.Count;

            return new ResumoMensalDto
            {
                RelatorioGastosDoMes = GetRelatorioDeGastosDoMes(
                    mesAtual,
                    categoriaIds,
                    despesasAtuais
                ),

                DespesasPorMembros = DistribuirDespesasEntreMembros(
                    despesaGeraisMaisAlmocoDividioPorMembro,
                    aluguelCondominioContaLuzPorMembroForaPeu,
                    aluguelCondominioContaLuzParaPeu,
                    totalAlmocoParteDoJhon
                )
            };
        }

        #region Support Methods

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

            return total.RoundTo(2);
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
            int idAlmoco
        )
        {
            double almoco = despesas.Where(d => d.CategoriaId == idAlmoco).Sum(d => d.Total);

            double almocoParteDoJhon = almoco / 5;

            double almocoAbatido = almoco - almocoParteDoJhon;

            return (almocoAbatido, almocoParteDoJhon);
        }

        private async Task<(double, double)> CalcularTotalAluguelCondominioContaDeLuzPorMembro(
            List<Despesa> despesas,
            CategoriaIdsDto categoriaIds
        )
        {
            var (idJhon, idPeu) = _membroRepository.GetIdsJhonPeu();

            List<Membro> todosMembrosDaCasa = await _membroRepository
                .Get(membro => membro.Id != idJhon)
                .ToListAsync();

            List<Membro> membrosMenosPeu = todosMembrosDaCasa
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
                (valorCondominio + valorContaDeLuz - 100) / todosMembrosDaCasa.Count; //100 reais referente ao estacionamento que alugamos.

            double aluguelPorMembroForaPeu = (valorAluguel - 300) / membrosMenosPeu.Count; //300 reais é o valor do aluguel do peu.

            double aluguelCondominioContaLuzPorMembroForaPeu =
                aluguelPorMembroForaPeu + luzMaisCondominioPorMembro;

            double aluguelCondominioContaLuzParaPeu = 300 + luzMaisCondominioPorMembro;

            return (aluguelCondominioContaLuzPorMembroForaPeu, aluguelCondominioContaLuzParaPeu);
        }

        private IEnumerable<DespesaPorMembroDto> DistribuirDespesasEntreMembros(
            double despesaGeraisMaisAlmocoDividioPorMembro,
            double aluguelCondominioContaLuzPorMembroForaPeu,
            double aluguelCondominioContaLuzParaPeu,
            double totalAlmocoDividioComJhon
        )
        {
            var members = _membroRepository.Get().ToList();
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

                ValorDespesasCasa =
                    member.Id == idJhon
                        ? totalAlmocoDividioComJhon.RoundTo(2)
                        : despesaGeraisMaisAlmocoDividioPorMembro.RoundTo(2),

                ValorCondominioAluguelContaDeLuz =
                    member.Id == idJhon ? 0 : ValorCondominioAluguelContaDeLuz(member)
            });

            return valoresPorMembro;
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
