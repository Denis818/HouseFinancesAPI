namespace Domain.Dtos.Despesas
{
    public record RelatorioGastosDoMesDto(
        string MesAtual,
        double TotalAluguelCondominio,
        double TotalGastosGerais,
        double TotalGeral
    );
}
