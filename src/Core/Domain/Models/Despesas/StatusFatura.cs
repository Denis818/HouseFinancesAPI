using System.Text.Json.Serialization;

namespace Domain.Models.Despesas
{
    public class StatusFatura
    {
        public int Id { get; set; }
        public string FaturaNome { get; set; }
        public string Estado { get; set; }

        [JsonIgnore]
        public int GrupoFaturaId { get; set; }

        [JsonIgnore]
        public GrupoFatura GrupoFatura { get; set; }
    }
}
