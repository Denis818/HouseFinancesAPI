using Domain.Models.Despesas;
using Domain.Models.Membros;

namespace Domain.Dtos.Despesas
{
    public class DetalhamentoDespesasHabitacionalDto
    {
        public DistribuicaoCustosHabitacionalDto DistribuicaoCustos { get; set; }
        public IEnumerable<Despesa> ListAluguel { get; set; }
        public List<Membro> ListMembroForaJhon { get; set; }
        public List<Membro> ListMembroForaJhonPeu { get; set; }

        public double ParcelaApartamento { get; set; }
        public double ParcelaCaixa { get; set; }
        public double ContaDeLuz { get; set; }
        public double Condominio { get; set; }
        public int IdPeu { get; set; }

    }
}