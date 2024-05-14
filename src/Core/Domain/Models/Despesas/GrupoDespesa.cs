using System.Text.Json.Serialization;

namespace Domain.Models.Despesas
{
    public class GrupoDespesa
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        [JsonIgnore]
        public ICollection<Despesa> Despesas { get; set; } = [];
    }
}
