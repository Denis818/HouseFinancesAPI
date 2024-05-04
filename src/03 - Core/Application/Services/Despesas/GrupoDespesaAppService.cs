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
        : BaseAppService<GrupoDespesa, IGrupoDespesaRepository>(service), IGrupoDespesaAppService
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
    }
}
