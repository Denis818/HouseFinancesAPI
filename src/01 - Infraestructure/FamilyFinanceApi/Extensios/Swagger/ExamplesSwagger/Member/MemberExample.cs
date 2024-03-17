using Application.Utilities;
using Domain.Models.Finance;
using ProEventos.API.Controllers.Base;
using Swashbuckle.AspNetCore.Filters;

namespace FamilyFinanceApi.Extensios.Swagger.ExamplesSwagger
{
    public class MemberExample : IExamplesProvider<ResponseResultDTO<Member>>
    {
        public ResponseResultDTO<Member> GetExamples()
        {
            return new ResponseResultDTO<Member>
            {
                Dados = new()
                {
                    Id = 1,
                    Nome = "Larissa"
                },

                Mensagens = [new Notificacao("")]
            };
        }
    }
}
