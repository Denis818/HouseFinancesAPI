namespace Domain.Models.Despesas
{
    public class StatusFatura
    {
        public int Id { get; set; }
        public string FaturaNome { get; set; }
        public string Estado { get; set; }

        public int GrupoFaturaId { get; set; }
        public GrupoFatura GrupoFatura { get; set; }
    }
}
