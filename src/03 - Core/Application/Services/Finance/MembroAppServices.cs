using Application.Constants;
using Application.Interfaces.Services.Finance;
using Application.Services.Base;
using Domain.Dtos.Membro;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Finance;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Finance
{
    public class MembroAppServices(IServiceProvider service)
        : BaseAppService<Membro, IMembroRepository>(service),
            IMembroAppServices
    {
        public async Task<IEnumerable<Membro>> GetAllAsync() =>
            await _repository.Get().OrderBy(c => c.Nome).ToListAsync();

        public async Task<Membro> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<Membro> InsertAsync(MembroDto membroDto)
        {
            membroDto.Nome = membroDto.Nome.Trim();

            if(Validator(membroDto))
                return null;

            if(await _repository.ExisteAsync(membroDto.Nome) != null)
                Notificar(EnumTipoNotificacao.ClientError, $"O membro {membroDto.Nome} já existe.");

            var membro = _mapper.Map<Membro>(membroDto);

            await _repository.InsertAsync(membro);

            if(!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.InsertError);
                return null;
            }

            return membro;
        }

        public async Task<Membro> UpdateAsync(int id, MembroDto membroDto)
        {
            membroDto.Nome = membroDto.Nome.Trim();

            if(Validator(membroDto))
                return null;

            var membro = await _repository.GetByIdAsync(id);

            if(membro is null)
            {
                Notificar(EnumTipoNotificacao.ClientError, ErrorMessages.NotFoundById + id);
                return null;
            }

            if(_repository.ValidaMembroParaAcao(membro.Id))
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    "Esse membro faz parta da regra de negócio. Não pode ser alterado."
                );
                return null;
            }

            if(await _repository.ExisteAsync(membroDto.Nome) is Membro membroExiste)
            {
                if(membro.Id != membroExiste.Id)
                {
                    Notificar(
                        EnumTipoNotificacao.ClientError,
                        $"Categoria {membroDto.Nome} já existe."
                    );
                    return null;
                }
            }

            _mapper.Map(membroDto, membro);

            _repository.Update(membro);

            if(!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.UpdateError);
                return null;
            }

            return membro;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var membro = await _repository.GetByIdAsync(id);

            if(membro == null)
            {
                Notificar(EnumTipoNotificacao.ClientError, ErrorMessages.NotFoundById + id);
                return false;
            }

            if(_repository.ValidaMembroParaAcao(membro.Id))
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    "Esse membro faz parta da regra de negócio. Não pode ser alterado."
                );
                return false;
            }

            _repository.Delete(membro);

            if(!await _repository.SaveChangesAsync())
            {
                Notificar(EnumTipoNotificacao.ServerError, ErrorMessages.DeleteError);
                return false;
            }

            Notificar(EnumTipoNotificacao.Informacao, "Registro Deletado");
            return true;
        }
    }
}
