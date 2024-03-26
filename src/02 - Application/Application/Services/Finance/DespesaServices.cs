using Application.Configurations.Extensions.Help;
using Application.Constants;
using Application.Interfaces.Services;
using Application.Services.Base;
using Domain.Dtos.Finance;
using Domain.Enumeradores;
using Domain.Interfaces;
using Domain.Models;
using FamilyFinanceApi.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Finance
{
    public class DespesaServices(IServiceProvider Service,
        IMemberServices _memberServices,
        ICategoriaServices _categoriaServices) :
        ServiceAppBase<Despesa, DespesaDto, IDespesaRepository>(Service), IDespesaServices
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

            if (!await _categoriaServices.Existe(despesaDto.CategoriaId))
            {
                Notificar(EnumTipoNotificacao.ClientError, "Categoria não existe.");
                return null;
            }

            var despesa = MapToModel(despesaDto);

            despesa.Total = despesa.Preco * despesa.Quantidade;

            await _repository.InsertAsync(despesa);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.InsertError);
                return null;
            }

            return await GetByIdAsync(despesa.Id);
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

            MapDtoToModel(despesaDto, despesa);

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

        public async Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoria()
        {
            (var inicioDoMes, var fimDoMes) = await GetPeriodoParaCalculo();

            var listDespesas = _repository.Get(d => d.DataCompra >= inicioDoMes && d.DataCompra <= fimDoMes)
                                          .Include(c => c.Categoria);

            var listAgrupada = listDespesas.GroupBy(despesa => despesa.Categoria.Descricao);

            return await listAgrupada.Select(list => 
                new DespesasTotalPorCategoria(list.Key, list.Sum(despesa => despesa.Total))).ToListAsync();
        }


        public async Task<DespesasPorMembroDto> GetTotalParaCadaMembro()
        {
            int members = await _memberServices.GetAllAsync().CountAsync();

            (var inicioDoMes, var fimDoMes) = await GetPeriodoParaCalculo();
            var mesDespesaAtual = inicioDoMes.ToString("Y");

            var totalDoMes = await _repository.Get()
                                              .Where(d => d.DataCompra >= inicioDoMes && d.DataCompra <= fimDoMes)
                                              .SumAsync(d => d.Total);

            var totalDespesaForaAlmoco = await _repository.Get(despesa => despesa.CategoriaId != 1)
                                                          .Where(d => d.DataCompra >= inicioDoMes && d.DataCompra <= fimDoMes)
                                                          .SumAsync(d => d.Total);

            var totalAlmocoAbateJhon = await _repository.Get(despesa => despesa.CategoriaId == 1)
                                                        .Where(d => d.DataCompra >= inicioDoMes && d.DataCompra <= fimDoMes)
                                                        .SumAsync(d => d.Total);



            totalAlmocoAbateJhon -= totalAlmocoAbateJhon / 5;

            var totalPorMembro = (totalDespesaForaAlmoco + totalAlmocoAbateJhon) / members;

            return new DespesasPorMembroDto(totalPorMembro.RoundTo(2), totalDoMes, mesDespesaAtual);
        }

        public async Task<PagedResult<DespesasPorMesDto>> GetTotaisComprasPorMesAsync(int paginaAtual, int itensPorPagina)
        {
            var despesasPorMes = _repository.Get()
                                            .GroupBy(d => new { d.DataCompra.Month, d.DataCompra.Year })
                                            .Select(group => new DespesasPorMesDto(
                                                new DateTime(group.Key.Year, group.Key.Month, 1).ToString("Y"),
                                                group.Sum(d => d.Total).RoundTo(2)
                                            ));


            return await Pagination.PaginateResultAsync(despesasPorMes, paginaAtual, itensPorPagina);
        }

        #region Support Methods

        private async Task<(DateTime, DateTime)> GetPeriodoParaCalculo()
        {
            var dataMaisRecente = await _repository.Get()
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
