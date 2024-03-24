using Application.Utilities;
using Domain.Models;
using ProEventos.API.Controllers.Base;
using Swashbuckle.AspNetCore.Filters;

namespace FamilyFinanceApi.Extensios.Swagger.ExamplesSwagger
{
    public class DespesaExample : IExamplesProvider<ResponseResultDTO<Despesa>>
    {
        public ResponseResultDTO<Despesa> GetExamples()
        {
            return new ResponseResultDTO<Despesa>
            {
                Dados = new()
                {
                    Id = 1,
                    Categoria = new Categoria { Nome = "Almoço" },
                    Item = "Arroz",
                    Preco = 15.2m,
                    Quantidade = 3,
                    Fornecedor = "EPA",
                    Total = 45.6m,
                    DataCompra = new DateTime(2024, 01, 30, 14, 07, 00),
                },

                Mensagens = [new Notificacao("")]
            };
        }
    }
}
