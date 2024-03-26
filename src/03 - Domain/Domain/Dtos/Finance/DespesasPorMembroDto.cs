using System.Globalization;

namespace Domain.Dtos.Finance
{
    public record DespesasPorMembroDto(decimal TotalPorMembro, decimal TotalDoMes, string Mes);
}
