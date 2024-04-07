using Application.Interfaces.Utility;
using Domain.Enumeradores;

namespace Application.Utilities
{
    public class Notificador : INotificador
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
