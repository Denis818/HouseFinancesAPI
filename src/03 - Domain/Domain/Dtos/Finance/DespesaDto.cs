using Domain.Converters.DatesTimes;
using Domain.Models;
using Domain.Models.Dtos.Finance;
using System.Text.Json.Serialization;

namespace Domain.Dtos.Finance
{
    public class DespesaDto
    {
        public int CategoriaId { get; set; }
        public string Item { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public string Fornecedor { get; set; }

        [JsonConverter(typeof(ShortDateFormatConverter))]
        public DateTime DataCompra { get; set; }
    }
}
