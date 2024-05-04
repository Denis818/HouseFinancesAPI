using Application.Interfaces.Services.Despesas;
using Application.Resources.Messages;
using Application.Services.Base;
using Domain.Dtos.Despesas.Criacao;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Despesas
{
    public class GrupoDespesaAppService(IServiceProvider service)
        : BaseAppService<GrupoDespesa, IGrupoDespesaRepository>(service),
            IGrupoDespesaAppService
    {
        public async Task<IEnumerable<GrupoDespesa>> GetAllAsync() =>
            await _repository.Get().OrderBy(c => c.Nome).ToListAsync();

        public async Task<GrupoDespesa> InsertAsync(GrupoDespesaDto grupoDto)
        {
            grupoDto.Nome = grupoDto.Nome.Trim();

            if(Validator(grupoDto))
                return null;

            var existingGrupo = await _repository
                .Get(grupo => grupo.Nome == grupoDto.Nome)
                .FirstOrDefaultAsync();

            if(existingGrupo != null)
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    string.Format(Message.RegistroExistente, "o Grupo", grupoDto.Nome)
                );
                return null;
            }

            var grupoDespesa = _mapper.Map<GrupoDespesa>(grupoDto);
            await _repository.InsertAsync(grupoDespesa);

            if(!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Inserir")
                );
                return null;
            }

            return grupoDespesa;
        }

        public async Task<GrupoDespesa> UpdateAsync(int id, GrupoDespesaDto grupoDespesaDto)
        {
            if(Validator(grupoDespesaDto))
                return null;

            var grupoDespesa = await _repository.GetByIdAsync(id);

            if(grupoDespesa is null)
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    string.Format(Message.IdNaoEncontrado, "Grupo Despesa", id)
                );
                return null;
            }

            if(grupoDespesa.Nome == grupoDespesaDto.Nome)
                return grupoDespesa;

            if(
                await _repository.ExisteAsync(nome: grupoDespesaDto.Nome)
                is GrupoDespesa grupoDespesaExiste
            )
            {
                if(grupoDespesa.Id != grupoDespesaExiste.Id)
                {
                    Notificar(
                        EnumTipoNotificacao.ClientError,
                        string.Format(
                            Message.RegistroExistente,
                            "O Grupo de Despesa",
                            grupoDespesaDto.Nome
                        )
                    );
                    return null;
                }
            }

            _mapper.Map(grupoDespesaDto, grupoDespesa);

            _repository.Update(grupoDespesa);

            if(!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Atualizar")
                );
                return null;
            }

            return grupoDespesa;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var grupoDespesa = await _repository.GetByIdAsync(id);

            if(grupoDespesa == null)
            {
                Notificar(
                    EnumTipoNotificacao.ClientError,
                    string.Format(Message.IdNaoEncontrado, "Grupo Despesa", id)
                );
                return false;
            }

            _repository.Delete(grupoDespesa);

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
