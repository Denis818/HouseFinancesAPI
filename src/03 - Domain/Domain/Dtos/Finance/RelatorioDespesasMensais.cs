using Domain.Dtos.Finance.Records;

namespace Domain.Dtos.Finance
{
    public record RelatorioDespesasMensais(
        string Mes,
        RelatorioGastosDoMesDto RelatorioGastosDoMes,
        IEnumerable<DespesaPorMembroDto> DespesasPorMembros
    );  
}
