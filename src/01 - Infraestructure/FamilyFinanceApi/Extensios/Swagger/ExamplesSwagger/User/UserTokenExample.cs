using Application.Utilities;
using Domain.Dtos.User;
using ProEventos.API.Controllers.Base;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Vendas.Extensios.Swagger.ExamplesSwagger.User
{
    public class UserTokenExample : IExamplesProvider<ResponseResultDTO<UserTokenDto>>
    {
        public ResponseResultDTO<UserTokenDto> GetExamples()
        {
            return new ResponseResultDTO<UserTokenDto>
            {
                Dados = new()
                {
                    Authenticated = true,
                    Expiration = new DateTime(2022, 12, 30, 15, 21, 54),
                    Token = "Seu Token",
                },

                Mensagens = [new Notificacao("Data de expiração no formato UTC.")]
            };
        }
    }
}