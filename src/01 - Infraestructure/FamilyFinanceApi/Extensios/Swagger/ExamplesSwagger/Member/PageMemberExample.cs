using Application.Utilities;
using Domain.Models.Finance;
using FamilyFinanceApi.Utilities;
using ProEventos.API.Controllers.Base;
using Swashbuckle.AspNetCore.Filters;

namespace FamilyFinanceApi.Extensios.Swagger.ExamplesSwagger
{
    public class PageMemberExample : IExamplesProvider<ResponseResultDTO<PagedResult<Member>>>
    {
        public ResponseResultDTO<PagedResult<Member>> GetExamples()
        {
            return new ResponseResultDTO<PagedResult<Member>>
            {
                Dados = new()
                {
                    TotalItens = 2,
                    PaginaAtual = 1,
                    Itens =
                    [
                        new()
                        {
                            Id = 1,
                            Nome = "João",
                        },
                        new()
                        {
                            Id = 2,
                            Nome = "Larissa",
                        },
                        new()
                        {
                            Id = 3,
                            Nome = "Jorge",
                        }
                    ],
                },

                Mensagens = [new Notificacao("")]
            };
        }

    }
}
