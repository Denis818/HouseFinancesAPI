namespace Domain.Dtos.Finance.Records
{
    public record RelatorioGastosDoMesDto(
        decimal TotalAluguelCondominio,
        decimal TotalGastosGerais,
        decimal TotalGeral);
}
