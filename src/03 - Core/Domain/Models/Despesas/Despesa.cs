using Domain.Converters.DatesTimes;
using Domain.Models.Categorias;
using System.Text.Json.Serialization;

namespace Domain.Models.Despesas
{
    public class Despesa
    {
        public int Id { get; set; }

        [JsonConverter(typeof(LongDateFormatConverter))]
        public DateTime DataCompra { get; set; }
        public string Item { get; set; }
        public double Preco { get; set; }
        public int Quantidade { get; set; }
        public string Fornecedor { get; set; }
        public double Total { get; set; }
        public Categoria Categoria { get; set; }

        [JsonIgnore]
        public int CategoriaId { get; set; }
    }
}
