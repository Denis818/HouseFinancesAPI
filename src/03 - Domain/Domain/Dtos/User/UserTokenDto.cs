using Domain.Converters;
using System.Text.Json.Serialization;

namespace Domain.Dtos.User
{
    public class UserTokenDto
    {
        public bool Authenticated { get; set; }

        [JsonConverter(typeof(TimeFormatConverter))]
        public DateTime Expiration { get; set; }
        public string Token { get; set; }
    }
}
