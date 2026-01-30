using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BildungsBericht.Models
{
    public class Fach
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [JsonPropertyName("fachName")]
        public string FachName { get; set; }

        [StringLength(255)]
        [JsonPropertyName("beschreibung")]
        public string? Beschreibung { get; set; }
    }
}
