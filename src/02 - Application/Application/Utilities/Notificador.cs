using Application.Interfaces.Utility;

namespace Application.Utilities
{
    public class Notificador : INotificador
    {
        public List<Notificacao> ListNotificacoes { get; } = new List<Notificacao>();

        public void Add(Notificacao notificacao)
        {
            ListNotificacoes.Add(notificacao);
        }

        public void Clear()
        {
            ListNotificacoes.Clear();
        }
    }
}
