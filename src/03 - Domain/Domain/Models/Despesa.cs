using Domain.Converters.DateTimes;
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class Despesa
    {
        public int Id { get; set; }
        public string Item { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public string Fornecedor { get; set; }
        public decimal Total { get; set; }
        public Categoria Categoria { get; set; }

        [JsonIgnore]
        public int CategoriaId { get; set; }

        [JsonConverter(typeof(LongDateFormatConverter))]
        public DateTime DataCompra { get; set; }
    }

}
