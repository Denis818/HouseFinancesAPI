using System.Text.Json.Serialization;
using Domain.Converters.DatesTimes;

namespace Domain.Dtos.Finance
{
    public class DespesaDto
    {
        public int CategoriaId { get; set; }
        public string Item { get; set; }
        public double Preco { get; set; }
        public int Quantidade { get; set; }
        public string Fornecedor { get; set; }

        [JsonConverter(typeof(ShortDateFormatConverter))]
        public DateTime DataCompra { get; set; }
    }
}
