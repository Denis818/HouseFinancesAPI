using Application.Utilities;

namespace Application.Interfaces.Utility
{
    public interface INotificador
    {
        List<Notificacao> ListNotificacoes { get; }
        void Add(Notificacao notificacao);
        void Clear();
    }
}
