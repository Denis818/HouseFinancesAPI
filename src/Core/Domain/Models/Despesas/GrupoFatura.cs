using System.Text.Json.Serialization;

namespace Domain.Models.Despesas
{
    public class GrupoFatura
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        [JsonIgnore]
        public ICollection<Despesa> Despesas { get; set; } = [];
        public ICollection<StatusFatura> StatusFaturas { get; set; } = [];
    }

}
