using Domain.Models.Despesas;
using Domain.Utilities;

namespace Domain.Dtos.Despesas.Consultas
{
    public class DespesasPorFornecedorDto
    {
        public string MediaDeFornecedor { get; set; }
        public PagedResult<Despesa> ItensDesteFornecedor { get; set; }
    }
}
