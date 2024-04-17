using Application.Utilities;
using Domain.Enumeradores;

namespace Application.Interfaces.Utilities
{
    public interface INotifier
    {
        List<Notificacao> ListNotificacoes { get; }
        void Notificar(EnumTipoNotificacao tipo, string message);
        void Clear();
    }
}
