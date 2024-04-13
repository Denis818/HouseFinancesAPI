namespace Domain.Dtos.Finance
{
    public class ResumoMensalDto
    {
        public RelatorioGastosDoMesDto RelatorioGastosDoMes { get; set; }
        public IEnumerable<DespesaPorMembroDto> DespesasPorMembros { get; set; }
    }
}
