using Application.Interfaces.Utilities;
using Domain.Enumeradores;

namespace Application.Utilities
{
    public class Notifier : INotifier
    {
        public List<Notificacao> ListNotificacoes { get; } = new List<Notificacao>();

        public void Clear()
        {
            ListNotificacoes.Clear();
        }

        public void Notificar(EnumTipoNotificacao tipo, string message)
            => ListNotificacoes.Add(new Notificacao(message, tipo));
    }
}
