using Domain.Enumeradores;
using Domain.Utilities;

namespace Domain.Dtos.BaseResponse
{
    public class ResponseDTO<T>
    {
        public T Dados { get; set; }
        public Notificacao[] Mensagens { get; set; }

        public ResponseDTO(T data, Notificacao[] messages = null)
        {
            Dados = data;
            Mensagens = messages ?? [];
        }

        public ResponseDTO() { }

        public void ContentTypeInvalido()
        {
            Mensagens = [new("Content-Type inválido.", EnumTipoNotificacao.ClientError)];
        }
    }
}
