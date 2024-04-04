namespace Domain.Dtos.Finance
{
    public record ResumoMensalDto(
        RelatorioGastosDoMesDto RelatorioGastosDoMes,
        IEnumerable<DespesaPorMembroDto> DespesasPorMembros
    );
}
