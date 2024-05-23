using Domain.Models.Despesas;

namespace Domain.Dtos.Despesas.Consultas
{
    public class SugestaoEconomiaDespesa
    {
        public string SugestaoDeFornecedor { get; set; }
        public IEnumerable<Despesa> ItensDesteFornecedor { get; set; } = [];
    }
}
