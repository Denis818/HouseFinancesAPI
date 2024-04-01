namespace Domain.Dtos.Finance
{
    public class RelatorioDespesasMensais()
    {
        public decimal TotalDoMes { get; set; }
        public string Mes { get; set; }
        public List<DespesaPorMembro> DespesasPorMembros { get; set; }
    }

    public class DespesaPorMembro
    {
        public string Nome { get; set; }
        public decimal Valor { get; set; }
    }
}
