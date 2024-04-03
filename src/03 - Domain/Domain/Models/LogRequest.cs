using Domain.Converters.DatesTimes;
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class LogRequest
    {
        public int Id { get; set; }
        public string TypeLog { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }

        [JsonConverter(typeof(ShortDateFormatConverter))]
        public DateTime InclusionDate { get; set; }

        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
    }
}
