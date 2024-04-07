using Application.Configurations.Extensions.Help;
using Application.Constants;
using Application.Interfaces.Services.Finance;
using Application.Services.Base;
using AutoMapper.Execution;
using Data.Repository.Finance;
using Domain.Dtos.Finance;
using Domain.Enumeradores;
using Domain.Interfaces;
using Domain.Models.Finance;
using Domain.Services;
using HouseFinancesAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders.Testing;
using System.Globalization;

namespace Application.Services.Finance
{
    public class DespesaServices(IServiceProvider service,
        IFinanceServices _financeServices,
        IMembroRepository _membroRepository,
        ICategoriaRepository _categoriaRepository) : BaseService<Despesa, IDespesaRepository>(service), IDespesaServices
    {
        #region CRUD
        public async Task<Despesa> GetByIdAsync(int id)
        {
            return await _repository.Get(despesa => despesa.Id == id)
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
            if (Validator(despesaDto)) return null;

            if (await _categoriaRepository.ExisteAsync(despesaDto.CategoriaId) is null)
            {
                Notificar(EnumTipoNotificacao.ClientError, $"Categoria com id:{despesaDto.CategoriaId} não existe.");
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

        public async Task<IEnumerable<Despesa>> InsertRangeAsync(IAsyncEnumerable<DespesaDto> listDespesasDto)
        {
            int totalRecebido = 0;
            var despesasParaInserir = new List<Despesa>();

            await foreach (var despesaDto in listDespesasDto)
            {
                totalRecebido++;

                if (Validator(despesaDto)) continue;

                if (await _categoriaRepository.ExisteAsync(despesaDto.CategoriaId) is null)
                {
                    Notificar(EnumTipoNotificacao.Informacao, $"Categoria com id:{despesaDto.CategoriaId} não existe.");
                    continue;
                }

                var despesa = _mapper.Map<Despesa>(despesaDto);
                despesa.Total = (despesa.Preco * despesa.Quantidade).RoundTo(2);
                despesasParaInserir.Add(despesa);
            }

            if (despesasParaInserir.Count == 0)
            {
                Notificar(EnumTipoNotificacao.ClientError, "Nunhuma das despesa é valida para inserir.");
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
                Notificar(EnumTipoNotificacao.Informacao,
                    $"{despesasParaInserir.Count} de {totalRecebido} despesas foram inseridas. " +
                    $"total de {totalRecebido - despesasParaInserir.Count} invalidas.");
            }

            var ids = despesasParaInserir.Select(d => d.Id).ToList();
            var despesasInseridas = await _repository.Get(d => ids
                                                     .Contains(d.Id))
                                                     .Include(c => c.Categoria)
                                                     .ToListAsync();

            return despesasInseridas;
        }

        public async Task<Despesa> UpdateAsync(int id, DespesaDto despesaDto)
        {
            if (Validator(despesaDto)) return null;

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

            var listDespesas = _repository.Get(d => d.DataCompra >= inicioDoMes && d.DataCompra <= fimDoMes)
                                          .Include(c => c.Categoria);

            var listAgrupada = listDespesas.GroupBy(despesa => despesa.Categoria.Descricao);

            return await listAgrupada.Select(list =>
                new DespesasTotalPorCategoria(list.Key, list.Sum(despesa => despesa.Total))).ToListAsync();
        }

        public async Task<IEnumerable<DespesasPorMesDto>> GetTotaisComprasPorMesAsync()
        {
            var despesasPorMes = _repository.Get()
                                            .GroupBy(d => new { d.DataCompra.Month, d.DataCompra.Year })
                                            .Select(group => new DespesasPorMesDto(
                                                new DateTime(group.Key.Year, group.Key.Month, 1).ToString("MMMM", new CultureInfo("pt-BR")),
                                                group.Sum(d => d.Total).RoundTo(2)
                                            ));


            return await despesasPorMes.ToListAsync();
        }

        public async Task<ResumoMensalDto> GetResumoDespesasMensalAsync()
        {
            var (idAlmoco, idAluguel) = _categoriaRepository.GetIdsAluguelAlmoco();
            var (inicioDoMes, fimDoMes) = await GetPeriodoParaCalculoAsync();



            List<Membro> listMembersForaJhon = await _membroRepository
                                           .Get(m => m.Nome != "Jhon Lenon").ToListAsync();

            string mesAtual = inicioDoMes.ToString("Y", new CultureInfo("pt-BR"));

            List<Despesa> despesasAtuais = await _repository.Get(d => d.DataCompra >= inicioDoMes &&
                                                                      d.DataCompra <= fimDoMes)
                                                            .Include(c => c.Categoria)
                                                            .ToListAsync();

            decimal totalDespesaForaAlmocoAluguel =
                _financeServices.CalculaTotalDespesaForaAlmocoAluguel(despesasAtuais, idAlmoco, idAluguel).RoundTo(2);

            decimal totalAluguelParaMembros =
                CalculaTotalAluguelParaMembros(despesasAtuais, idAluguel).RoundTo(2);

            var (totalAlmocoDividioComJhon, totalAlmocoParteDoJhon) =
                _financeServices.CalculaTotalAlmocoDivididoComJhon(despesasAtuais, idAlmoco);

            decimal totalDespesaForaAluguel = totalDespesaForaAlmocoAluguel + totalAlmocoDividioComJhon;
            decimal despesaPorMembroForaAluguel = (totalDespesaForaAluguel - 100) / listMembersForaJhon.Count; //desconto do estacionamento que alugamos



            return new ResumoMensalDto(
                _financeServices.GetRelatorioDeGastosDoMes(mesAtual, despesasAtuais),
                DistribuirDespesasEntreMembros(despesaPorMembroForaAluguel, totalAluguelParaMembros, totalAlmocoParteDoJhon)
            );
        }


        #region Support Methods

        private async Task<(DateTime, DateTime)> GetPeriodoParaCalculoAsync()
        {
            var dataMaisRecente = await _repository.Get()
                                                 .OrderByDescending(d => d.DataCompra)
                                                 .Select(d => d.DataCompra)
                                                 .FirstOrDefaultAsync();

            var inicioDoMes = new DateTime(dataMaisRecente.Year, dataMaisRecente.Month, 1);
            var fimDoMes = inicioDoMes.AddMonths(1).AddDays(-1);

            return (inicioDoMes, fimDoMes);
        }

        private decimal CalculaTotalAluguelParaMembros(List<Despesa> despesas, int idAluguel)
        {
            var (idJhon, idPeu) = _membroRepository.GetIdsJhonPeu();

            var membros = _membroRepository.Get(membro => membro.Id != idJhon &&
                                                         membro.Id != idPeu).ToList();

            var totalAluguel = despesas.Where(d => d.CategoriaId == idAluguel)
                                       .Sum(d => d.Total);

            return (totalAluguel - 300) / membros.Count;
        }

        private IEnumerable<DespesaPorMembroDto> DistribuirDespesasEntreMembros(
            decimal despesaPorMembroForaAluguel,
            decimal totalAluguelPara3Membros,
            decimal totalAlmocoDividioComJhon)
        {
            var members = _membroRepository.Get().ToList();
            var (idJhon, idPeu) = _membroRepository.GetIdsJhonPeu();


            decimal CalculaValorPorMembro(Membro membro)
            {
                if (membro.Id == idPeu)
                {
                    return despesaPorMembroForaAluguel + 300;
                }
                else if (membro.Id == idJhon)
                {
                    return totalAlmocoDividioComJhon;
                }
                else
                {
                    return despesaPorMembroForaAluguel + totalAluguelPara3Membros;
                }
            }

            return members.Select(member =>
                    new DespesaPorMembroDto(member.Nome, CalculaValorPorMembro(member).RoundTo(2)));
        }  
        #endregion
    }
}
