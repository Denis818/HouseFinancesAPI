namespace Domain.Dtos.Despesas.Relatorios
{
    public record RelatorioGastosDoGrupoDto
    {
        public string GrupoDespesaNome { get; set; }
        public double TotalGastosMoradia { get; set; }
        public double TotalGastosCasa { get; set; }
        public double TotalGeral { get; set; }
    }
}
