using Domain.Enumeradores;

namespace Application.Utilities
{
    public class Notificacao(string mensagem, EnumTipoNotificacao tipo = EnumTipoNotificacao.Informacao)
    {
        public EnumTipoNotificacao StatusCode { get; set; } = tipo;
        public string Descricao { get; set; } = mensagem;
    }
}
