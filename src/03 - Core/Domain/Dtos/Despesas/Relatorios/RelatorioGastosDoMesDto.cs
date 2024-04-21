namespace Domain.Dtos.Despesas.Relatorios
{
    public record RelatorioGastosDoMesDto(
        string MesAtual,
        double TotalAluguelCondominio,
        double TotalGastosGerais,
        double TotalGeral
    );
}
