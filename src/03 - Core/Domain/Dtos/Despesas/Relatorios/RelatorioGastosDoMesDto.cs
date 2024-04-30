namespace Domain.Dtos.Despesas.Relatorios
{
    public record RelatorioGastosDoMesDto
    {
        public string MesAtual { get; set; }
        public double TotalGastosMoradia { get; set; }
        public double TotalGastosCasa { get; set; }
        public double TotalGeral { get; set; }
    }
}
