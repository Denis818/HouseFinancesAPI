using Application.Constants;
using Application.Extensions.Help;
using Application.Interfaces.Services.Despesas;
using Application.Services.Base;
using Application.Services.Despesas.RelatorioPdf;
using Application.Utilities;
using Domain.Converters.DatesTimes;
using Domain.Dtos.Despesas.Criacao;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Despesas
{
    public class DespesaAppServices(
        IServiceProvider service,
        IDespesaConsultaAppService _despesaConsultaApp,
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
                        EnumTipoNotificacao.ClientError,
                        $"Categoria com id:{despesaDto.CategoriaId} não existe."
                    );
                    continue;
                }

                var despesa = _mapper.Map<Despesa>(despesaDto);
                despesa.Total = (despesa.Preco * despesa.Quantidade).RoundTo(2);

                despesa.DataCompra = DateTimeZoneProvider.GetBrasiliaTimeZone(DateTime.UtcNow);
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
            despesa.DataCompra = DateTimeZoneProvider.GetBrasiliaTimeZone(DateTime.UtcNow);


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

        public async Task<byte[]> DownloadPdfRelatorioDeDespesaHabitacional()
        {
            var custosHabitacionalDto =
                await _despesaConsultaApp.CalcularDistribuicaoCustosHabitacionalAsync();

            return new DespesaHabitacionalPdfAppService().GerarRelatorioDespesaHabitacionalPdf(
                custosHabitacionalDto
            );
        }

        public async Task<byte[]> DownloadPdfRelatorioDeDespesaCasa()
        {
            var custosCasaDto = await _despesaConsultaApp.CalcularDistribuicaoCustosCasaAsync();

            return new DespesaCasaPdfAppService().GerarRelatorioDespesaCasaPdf(custosCasaDto);
        }
    }
}
