using Domain.Converters.DatesTimes;
using System.Text.Json.Serialization;

namespace Domain.Models.Membros
{
    public class Membro
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Telefone { get; set; }

        [JsonConverter(typeof(LongDateFormatConverter))]
        public DateTime DataInicio { get; set; }
    }
}
