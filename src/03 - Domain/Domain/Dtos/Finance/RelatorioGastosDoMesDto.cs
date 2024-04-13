namespace Domain.Dtos.Finance
{
    public record RelatorioGastosDoMesDto(
        string MesAtual,
        double TotalAluguelCondominio,
        double TotalGastosGerais,
        double TotalGeral
    );
}
