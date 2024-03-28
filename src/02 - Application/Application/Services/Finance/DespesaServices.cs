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

        public async Task<IEnumerable<Despesa>> InsertRangeAsync(IAsyncEnumerable<DespesaDto> listDespesasDto)
        {
            var totalRecebido = 0;
            var despesasParaInserir = new List<Despesa>();

            await foreach (var despesaDto in listDespesasDto)
            {
                totalRecebido++;

                if (Validator(despesaDto)) continue; //As notificações já são tratadas dentro do Validator

                if (!await _categoriaServices.Existe(despesaDto.CategoriaId))
                {
                    Notificar(EnumTipoNotificacao.Informacao, $"Categoria com id:{despesaDto.CategoriaId} não existe.");
                    continue;
                }

                var despesa = MapToModel(despesaDto);
                despesa.Total = despesa.Preco * despesa.Quantidade;
                despesasParaInserir.Add(despesa);
            }

            if (despesasParaInserir.Count == 0) return [];

            await _repository.InsertRangeAsync(despesasParaInserir);
            if (!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.InsertError);
                return null;
            }

            Notificar(EnumTipoNotificacao.Informacao,
                $"{despesasParaInserir.Count} de {totalRecebido} despesas foram inseridas com sucesso. Algumas não eram validas.");

            var despesasInseridas = await _repository.Get(d => despesasParaInserir.Select(p => p.Id)
                                                     .Contains(d.Id))
                                                     .Include(c => c.Categoria)
                                                     .ToListAsync();

            return despesasInseridas;
        }


        public async Task<IEnumerable<DespesasTotalPorCategoria>> GetTotalPorCategoriaAsync()
        {
            var (inicioDoMes, fimDoMes) = await GetPeriodoParaCalculoAsync();

            var listDespesas = _repository.Get(d => d.DataCompra >= inicioDoMes && d.DataCompra <= fimDoMes)
                                          .Include(c => c.Categoria);

            var listAgrupada = listDespesas.GroupBy(despesa => despesa.Categoria.Descricao);

            return await listAgrupada.Select(list =>
                new DespesasTotalPorCategoria(list.Key, list.Sum(despesa => despesa.Total))).ToListAsync();
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

        public async Task<DespesasPorMembroDto> GetTotalParaCadaMembroAsync()
        {
            var listMembers = await _memberServices.GetAllAsync().ToListAsync();

            (int idAlmoco, int idAluguel) = await _categoriaServices.GetIdsAluguelAlmocoAsync();

            var (inicioDoMes, fimDoMes) = await GetPeriodoParaCalculoAsync();
            string mesDespesaAtual = inicioDoMes.ToString("Y");

            List<Despesa> despesasRecentes = await _repository.Get(d => d.DataCompra >= inicioDoMes &&
                                                                        d.DataCompra <= fimDoMes).ToListAsync();

            decimal totalDoMes = despesasRecentes.Sum(d => d.Total);

            decimal totalDespesaForaAlmocoAluguel = CalculaTotalDespesaForaAlmocoAluguel(despesasRecentes, idAlmoco, idAluguel).RoundTo(2);
            decimal totalAluguelPara3Membros = CalculaTotalAluguelPara3Membros(despesasRecentes, idAluguel).RoundTo(2);
            decimal totalAlmocoComAbateJhon = CalculaTotalAlmocoComAbateJhon(despesasRecentes, idAlmoco).RoundTo(2);

            decimal totalDespesaForaAluguel = totalDespesaForaAlmocoAluguel + totalAlmocoComAbateJhon;  
            decimal despesaPorMembroForaAluguel = totalDespesaForaAluguel / listMembers.Count - 100; //desconto do estacionamento que alugamos


            DespesasPorMembroDto valoresPorMembro = new()
            {
                Mes = mesDespesaAtual,
                TotalDoMes = totalDoMes,
                DespesasPorMembros = CalculaValoresPorMembro(listMembers, despesaPorMembroForaAluguel, totalAluguelPara3Membros)
            };

            return valoresPorMembro;
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
        #endregion

        #region Calculos para despesa por membros

        private decimal CalculaTotalDespesaForaAlmocoAluguel(
            List<Despesa> despesas,
            int idAlmoco,
            int idAluguel)
        {
            return despesas.Where(d => d.CategoriaId != idAlmoco &&
                                       d.CategoriaId != idAluguel).Sum(d => d.Total);
        }

        private decimal CalculaTotalAluguelPara3Membros(
            List<Despesa> despesas,
            int idAluguel)
        {
            var totalAluguel = despesas.Where(d => d.CategoriaId == idAluguel)
                                       .Sum(d => d.Total);

            return (totalAluguel - 300) / 3;
        }

        private decimal CalculaTotalAlmocoComAbateJhon(
            List<Despesa> despesas,
            int idAlmoco)
        {
            var totalAlmoco = despesas.Where(d => d.CategoriaId == idAlmoco)
                                      .Sum(d => d.Total);

            return totalAlmoco - (totalAlmoco / 5);
        }

        private List<DespesaPorMembro> CalculaValoresPorMembro(
            List<Member> members,
            decimal despesaPorMembroForaAluguel,
            decimal totalAluguelPara3Membros)
        {
            return members.Select(member => new DespesaPorMembro
            {
                Nome = member.Nome,
                Valor = member.Nome == "Peu" ? despesaPorMembroForaAluguel + 300 :
                                               despesaPorMembroForaAluguel + totalAluguelPara3Membros
            }).ToList();
        }
        #endregion
    }
}
