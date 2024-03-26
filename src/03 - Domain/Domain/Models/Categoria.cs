using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Descricao { get; set; }

        [JsonIgnore]
        public ICollection<Despesa> Despesas { get; set; } = [];
    }
}
