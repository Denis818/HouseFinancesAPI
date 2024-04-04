namespace Domain.Dtos.Finance
{
    public record ResumoMensalDto(
        string Mes,
        RelatorioGastosDoMesDto RelatorioGastosDoMes,
        IEnumerable<DespesaPorMembroDto> DespesasPorMembros
    );
}
