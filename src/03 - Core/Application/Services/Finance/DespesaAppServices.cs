using Application.Constants;
using Application.Extensions.Help;
using Application.Interfaces.Services.Finance;
using Application.Services.Base;
using Application.Utilities;
using Domain.Dtos.Finance;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services.Finance;
using Domain.Models.Finance;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Application.Services.Finance
{
    public class DespesaAppServices(
        IServiceProvider service,
        IDespesasDomainService _despesasDomainService,
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
            var query = _repository.Get().Include(c => c.Categoria).OrderBy(d => d.Id);
            return await Pagination.PaginateResultAsync(query, paginaAtual, itensPorPagina);
        }

        public async Task<Despesa> InsertAsync(DespesaDto despesaDto)
        {
            despesaDto.Item = despesaDto.Item.Trim();
            despesaDto.Fornecedor = despesaDto.Fornecedor.Trim();

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
            despesaDto.Item = despesaDto.Item.Trim();
            despesaDto.Fornecedor = despesaDto.Fornecedor.Trim();

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
            var (idJhon, idPeu) = _membroRepository.GetIdsJhonPeu();

            var (inicioDoMes, fimDoMes) = await GetPeriodoParaCalculoAsync();
            string mesAtual = inicioDoMes.ToString("Y", new CultureInfo("pt-BR"));

            List<Membro> listTodosMembros = await _membroRepository.Get().ToListAsync();

            List<Membro> listMembersForaJhon = await _membroRepository
                .Get(m => m.Id != idJhon)
                .ToListAsync();

            List<Despesa> despesasAtuais = await _repository
                .Get(d => d.DataCompra >= inicioDoMes && d.DataCompra <= fimDoMes)
                .Include(c => c.Categoria)
                .ToListAsync();

            //Despesas gerais Limpesa, Higiêne etc... Fora Almoço, alugeul, condominio e conta de luz.
            double totalDespesaGerais =
                _despesasDomainService.CalculaTotalDespesaForaAlmocoAluguelCondominioContaDeLuz(
                    despesasAtuais,
                    categoriaIds
                );

            //Aluguel + Condomínio + Conta de Luz
            var (aluguelCondominioContaLuzPorMembroForaPeu, aluguelCondominioContaLuzParaPeu) =
                _despesasDomainService.CalcularTotalAluguelCondominioContaDeLuzPorMembro(
                    despesasAtuais,
                    categoriaIds,
                    listTodosMembros,
                    idJhon,
                    idPeu
                );

            //Almoço divido com Jhon
            var (totalAlmocoDividioComJhon, totalAlmocoParteDoJhon) =
                _despesasDomainService.CalculaTotalAlmocoDivididoComJhon(
                    despesasAtuais,
                    categoriaIds.IdAlmoco,
                    listTodosMembros.Count
                );

            //Despesa gerais Limpesa, Higiêne etc... somado com Almoço divido com Jhon
            double despesaGeraisMaisAlmocoDividioPorMembro =
                (totalDespesaGerais + totalAlmocoDividioComJhon) / listMembersForaJhon.Count;

            return new ResumoMensalDto
            {
                RelatorioGastosDoMes = _despesasDomainService.GetRelatorioDeGastosDoMes(
                    mesAtual,
                    categoriaIds,
                    despesasAtuais
                ),

                DespesasPorMembros = DistribuirDespesasEntreMembros(
                    listTodosMembros,
                    despesaGeraisMaisAlmocoDividioPorMembro,
                    aluguelCondominioContaLuzPorMembroForaPeu,
                    aluguelCondominioContaLuzParaPeu,
                    totalAlmocoParteDoJhon
                )
            };
        }

        #endregion

        public byte[] DownloadPdfRelatorioDeDespesas()
        {
            var despesas = _repository.Get().ToList();
            var membros = _membroRepository.Get().ToList();
            var categoriaIds = _categoriaRepository.GetCategoriaIds();
            var (idJhon, idPeu) = _membroRepository.GetIdsJhonPeu();

            var t = new RelatorioDespesasPdfAppServices();
            return t.DownloadPdfRelatorioDeDespesas(despesas, membros, categoriaIds, idJhon, idPeu);
        }

        #region Support Methods

        private IEnumerable<DespesaPorMembroDto> DistribuirDespesasEntreMembros(
            List<Membro> members,
            double despesaGeraisMaisAlmocoDividioPorMembro,
            double aluguelCondominioContaLuzPorMembroForaPeu,
            double aluguelCondominioContaLuzParaPeu,
            double totalAlmocoDividioComJhon
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
