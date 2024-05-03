using Domain.Dtos.Despesas.Relatorios;
using Domain.Dtos.Despesas.Resumos;

namespace Domain.Dtos.Despesas.Consultas
{
    public class DespesasDivididasMensalDto
    {
        public RelatorioGastosDoMesDto RelatorioGastosDoMes { get; set; }
        public IEnumerable<DespesaPorMembroDto> DespesasPorMembro { get; set; }
    }
}
