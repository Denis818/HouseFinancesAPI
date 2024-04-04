namespace Domain.Dtos.Finance
{
    public record RelatorioGastosDoMesDto(
        string MesAtual,
        decimal TotalAluguelCondominio,
        decimal TotalGastosGerais,
        decimal TotalGeral);
}
