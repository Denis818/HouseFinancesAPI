using Application.Utilities;
using Domain.Dtos.Responses;
using FamilyFinanceApi.Utilities;
using ProEventos.API.Controllers.Base;
using Swashbuckle.AspNetCore.Filters;

namespace FamilyFinanceApi.Extensios.Swagger.ExamplesSwagger
{
    public class PageDespesabyMonthExample : IExamplesProvider<ResponseResultDTO<PagedResult<DespesasPorMesDto>>>
    {
        public ResponseResultDTO<PagedResult<DespesasPorMesDto>> GetExamples()
        {
            return new ResponseResultDTO<PagedResult<DespesasPorMesDto>>
            {
                Dados = new()
                {
                    TotalItens = 3,
                    PaginaAtual = 1,
                    Itens =
                    [
                        new()
                        {
                            Mes = "Janeiro De 2024",
                            TotalDespesas = 837.50m
                        },
                        new()
                        {
                            Mes = "fevereiro De 2024",
                            TotalDespesas = 337.50m
                        },
                         new()
                        {
                            Mes = "Março De 2024",
                            TotalDespesas = 537.50m
                        },
                    ],
                },

                Mensagens = [new Notificacao("")]
            };
        }

    }
}
