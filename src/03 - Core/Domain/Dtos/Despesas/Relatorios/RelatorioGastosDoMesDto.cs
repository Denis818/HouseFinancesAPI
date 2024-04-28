namespace Domain.Dtos.Despesas.Relatorios
{
    public record RelatorioGastosDoMesDto(
        string MesAtual,
        double TotalGastosHabitacional,
        double TotalGastosCasa,
        double TotalGeral
    );
}
