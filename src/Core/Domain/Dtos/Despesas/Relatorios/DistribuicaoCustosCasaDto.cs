using Domain.Models.Membros;

namespace Domain.Dtos.Despesas.Relatorios
{
    public class DistribuicaoCustosCasaDto
    {
        public List<Membro> Membros { get; set; }
        public double TotalDespesaGeraisForaAlmoco { get; set; }
        public double TotalAlmocoDividioComJhon { get; set; }
        public double TotalAlmocoParteDoJhon { get; set; }
        public double DespesaGeraisMaisAlmoco { get; set; }
        public double DespesaGeraisMaisAlmocoDividioPorMembro { get; set; }
    }

    public class CustosDespesasCasaDto
    {
        public List<Membro> TodosMembros { get; set; }
        public double ValorTotalAlmoco { get; set; }
        public double TotalDespesaGeraisForaAlmoco { get; set; }
        public int MembrosForaJhonCount { get; set; }
    }
}
