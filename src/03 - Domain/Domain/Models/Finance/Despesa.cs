using Domain.Converters;
using System.Text.Json.Serialization;

namespace Domain.Models.Finance
{
    public class Despesa
    {
        public int Id { get; set; }
        public string Categoria { get; set; }
        public string Item { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public string Fornecedor { get; set; }
        public decimal Total { get; set; }

        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime DataCompra { get; set; }
    }

}
