using Domain.Models.Despesas;
using Domain.Models.Membros;

namespace Domain.Dtos.Despesas.Relatorios
{
    public class CustosDespesasMoradiaDto
    {
        public double ParcelaApartamento { get; set; }
        public double ParcelaCaixa { get; set; }
        public double ContaDeLuz { get; set; }
        public double Condominio { get; set; }
    }

    public class GrupoListMembrosDespesaDto
    {
        public IList<Despesa> ListAluguel { get; set; } = [];
        public IList<Membro> ListMembroForaJhon { get; set; } = [];
        public IList<Membro> ListMembroForaJhonPeu { get; set; } = [];
    }

    public class DetalhamentoDespesasMoradiaDto
    {
        public CustosDespesasMoradiaDto CustosDespesasMoradia { get; set; } = new();
        public GrupoListMembrosDespesaDto GrupoListMembrosDespesa { get; set; } = new();
        public DistribuicaoCustosMoradiaDto DistribuicaoCustos { get; set; } = new();
    }
}
