namespace Domain.Dtos.Despesas.Relatorios
{
    public class DistribuicaoCustosMoradiaDto
    {
        public double TotalAptoMaisCaixa { get; set; }
        public double TotalLuzMaisCondominio { get; set; }
        public double TotalAptoMaisCaixaAbate300Peu100Estacionamento { get; set; }
        public double ValorAptoMaisCaixaParaCadaMembro { get; set; }
        public double ValorLuzMaisCondominioParaCadaMembro { get; set; }
        public double ValorParaMembrosForaPeu { get; set; }
        public double ValorParaDoPeu { get; set; }
    }
}
