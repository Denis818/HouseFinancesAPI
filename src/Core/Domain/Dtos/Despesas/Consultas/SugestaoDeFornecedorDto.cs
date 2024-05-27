using Domain.Models.Despesas;
using Domain.Utilities;

namespace Domain.Dtos.Despesas.Consultas
{
    public class SugestaoDeFornecedorDto
    {
        public string Sugestao { get; set; }
        public PagedResult<Despesa> ListaItens { get; set; }
    }
}
