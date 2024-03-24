using System.Globalization;

namespace Domain.Dtos.Finance
{
    public record DespesasMensaisPorMembroDto(decimal TotalPorMembro, decimal TotalDoMes, string Mes);
}
