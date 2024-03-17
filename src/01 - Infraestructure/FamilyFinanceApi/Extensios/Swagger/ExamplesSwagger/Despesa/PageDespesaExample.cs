using Application.Utilities;
using Domain.Models.Finance;
using FamilyFinanceApi.Utilities;
using ProEventos.API.Controllers.Base;
using Swashbuckle.AspNetCore.Filters;

namespace FamilyFinanceApi.Extensios.Swagger.ExamplesSwagger
{
    public class PageDespesaExample : IExamplesProvider<ResponseResultDTO<PagedResult<Despesa>>>
    {
        public ResponseResultDTO<PagedResult<Despesa>> GetExamples()
        {
            return new ResponseResultDTO<PagedResult<Despesa>>
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
                            Categoria = "Almoço",
                            Item = "Arroz",
                            Preco = 15.2m,
                            Quantidade = 3,
                            Fornecedor = "EPA",
                            Total = 45.6m,
                            DataCompra = new DateTime(2024, 01, 30, 14, 07, 00),
                        },
                        new()
                        {
                            Id = 2,
                            Categoria = "Cafe da manhã",
                            Item = "Pães",
                            Preco = 2.30m,
                            Quantidade = 7,
                            Fornecedor = "EPA",
                            Total = 16.1m,
                            DataCompra = new DateTime(2024, 01, 30, 14, 07, 00),
                        }
                    ],
                },

                Mensagens = [new Notificacao("")]
            };
        }

    }
}
