using Application.Resources.Messages;
using Application.Services.Base;
using Domain.Dtos.Despesas.Criacao;
using Domain.Enumeradores;
using Domain.Interfaces.Repositories;
using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class StatusFaturaAppService(IServiceProvider service)
        : BaseAppService<StatusFatura, IStatusFaturaRepository>(service),
            IStatusFaturaAppService
    {
        public async Task<StatusFaturaDto> GetStatusFaturaDtoByNameAsync(string status)
        {
            var statusFatura = await _repository.Get().FirstOrDefaultAsync(s => s.Estado == status);
            if(statusFatura == null)
            {
                var defaultState = status.Contains("Casa")
                    ? EnumStatusFatura.CasaFechado.ToString()
                    : EnumStatusFatura.MoradiaFechado.ToString();

                return new StatusFaturaDto { Estado = defaultState };
            }

            return new StatusFaturaDto { Estado = statusFatura.Estado };
        }

        public async Task<StatusFatura> UpdateAsync(EnumFaturaNome faturaNome, EnumStatusFatura status)
        {
            var statusFatura = await _repository
                .Get()
                .FirstOrDefaultAsync(s => s.FaturaNome == faturaNome.ToString());

            statusFatura.Estado = status.ToString();

            _repository.Update(statusFatura);

            if(!await _repository.SaveChangesAsync())
            {
                Notificar(
                    EnumTipoNotificacao.ServerError,
                    string.Format(Message.ErroAoSalvarNoBanco, "Atualizar")
                );
                return null;
            }

            return statusFatura;
        }
    }
}
