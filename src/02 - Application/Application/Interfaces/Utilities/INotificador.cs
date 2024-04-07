using Application.Utilities;
using Domain.Enumeradores;

namespace Application.Interfaces.Utility
{
    public interface INotificador
    {
        List<Notificacao> ListNotificacoes { get; }
        void Notificar(EnumTipoNotificacao tipo, string message);
        void Clear();
    }
}
