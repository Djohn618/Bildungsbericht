using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BildungsBericht.Models
{
    public class Rolle
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [JsonPropertyName("rolle")]
        public string RolleName { get; set; }

        [StringLength(255)]
        [JsonPropertyName("beschreibung")]
        public string? Beschreibung { get; set; }
    }
}
