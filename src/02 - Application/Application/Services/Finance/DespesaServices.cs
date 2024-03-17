using Application.Constants;
using Application.Interfaces.Services;
using Application.Services.Base;
using Domain.Converters;
using Domain.Dtos.Finance;
using Domain.Enumeradores;
using Domain.Interfaces.Repository.Finance;
using Domain.Models.Finance;
using FamilyFinanceApi.Utilities;

namespace Application.Services.Finance
{
    public class DespesaServices(IServiceProvider service) :
        ServiceAppBase<Despesa, DespesaDto, IDespesaRepository>(service), IDespesaServices
    {
        public async Task<PagedResult<Despesa>> GetAllDespesaAsync(int paginaAtual, int itensPorPagina)
            => await Pagination.PaginateResult(_repository.Get(), paginaAtual, itensPorPagina);

        public async Task<Despesa> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<Despesa> InsertAsync(DespesaDto despesaDto)
        {
            if (Validator(despesaDto)) return null;

            var despesa = MapToModel(despesaDto);

            despesa.Total = despesa.Preco * despesa.Quantidade;

            await _repository.InsertAsync(despesa);

            if (!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.InsertError);
                return null;
            }

            return despesa;
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

            return despesa;
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

    }
}
