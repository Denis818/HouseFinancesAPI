using Application.Interfaces.Services.Membros;
using Application.Resources.Messages;
using Application.Services.Base;
using Domain.Dtos.Membros;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Membros;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Membros
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
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    string.Format(Message.RegistroExistente, "O membro", membroDto.Nome)
                );
                return null;
            }

            var membro = _mapper.Map<Membro>(membroDto);

            await _repository.InsertAsync(membro);

            if(!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Inserir")
                );
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
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    string.Format(Message.IdNaoEncontrado, "O membro", id)
                );
                return null;
            }

            if(_repository.ValidaMembroParaAcao(membro.Id))
            {
                Notificar(EnumTipoNotificacao.ClientError, Message.AvisoMembroImutavel);
                return null;
            }

            if(await _repository.ExisteAsync(membroDto.Nome) is Membro membroExiste)
            {
                if(membro.Id != membroExiste.Id)
                {
                    Notificar(
                        EnumTipoNotificacao.ClientError,
                        string.Format(Message.RegistroExistente, "O membro", membroDto.Nome)
                    );
                    return null;
                }
            }

            _mapper.Map(membroDto, membro);

            _repository.Update(membro);

            if(!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Atualizar")
                );
                return null;
            }

            return membro;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var membro = await _repository.GetByIdAsync(id);

            if(membro == null)
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    string.Format(Message.IdNaoEncontrado, "O membro", id)
                );
                return false;
            }

            if(_repository.ValidaMembroParaAcao(membro.Id))
            {
                Notificar(EnumTipoNotificacao.ClientError, Message.AvisoMembroImutavel);
                return false;
            }

            _repository.Delete(membro);

            if(!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Deletar")
                );
                return false;
            }

            return true;
        }
    }
}
