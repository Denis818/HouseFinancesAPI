using Domain.Converters;
using System.Text.Json.Serialization;

namespace Domain.Models.Dtos
{
    public class DespesaDto
    {
        public string Categoria { get; set; }
        public string Item { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public string Fornecedor { get; set; }

        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime DataCompra { get; set; }
    }

}
