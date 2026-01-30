using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BildungsBericht.Models
{
    public class LehrBeruf
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [JsonPropertyName("lehrberufName")]
        public string LehrberufName { get; set; }

        [StringLength(255)]
        [JsonPropertyName("beschreibung")]
        public string? Beschreibung { get; set; }
    }
}
