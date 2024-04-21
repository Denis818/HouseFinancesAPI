using Domain.Models.Membros;

namespace Domain.Dtos.Despesas.Relatorios
{
    public class DistribuicaoCustosCasaDto
    {
        public int IdJhon { get; set; }
        public List<Membro> Membros { get; set; }
        public double TotalDespesaGerais { get; set; }
        public double TotalAlmocoDividioComJhon { get; set; }
        public double TotalAlmocoParteDoJhon { get; set; }
        public double DespesaGeraisMaisAlmoco { get; set; }
        public double DespesaGeraisMaisAlmocoDividioPorMembro { get; set; }
    }
}
