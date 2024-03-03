#nullable disable
using System.Text.Json.Serialization;

namespace FCxLabs.Application.DTOs
{
    public class ApplicationUserUpdateDTO : ApplicationUserDTO
    {
        [JsonIgnore]
        public string Id { get; set; }

    }
}
