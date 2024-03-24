using Domain.Converters;
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class Despesa
    {
        public int Id { get; set; }
        public Categoria Categoria { get; set; }
        public int CategoriaId { get; set; } // Chave estrangeira
        public string Item { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public string Fornecedor { get; set; }
        public decimal Total { get; set; }

        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime DataCompra { get; set; }
    }

}
