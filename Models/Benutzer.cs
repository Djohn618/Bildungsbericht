using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BildungsBericht.Models
{
    public class Benutzer
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [JsonPropertyName("vorname")]
        public string Vorname { get; set; }

        [Required]
        [StringLength(50)]
        [JsonPropertyName("nachname")]
        public string Nachname { get; set; }

        [JsonPropertyName("geburtsdatum")]
        public DateTime? Geburtsdatum { get; set; }

        [StringLength(100)]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [Required]
        [StringLength(45)]
        [JsonPropertyName("passwort")]
        public string Passwort { get; set; }

        [JsonPropertyName("rolleId")]
        public int RolleId { get; set; }

        [JsonPropertyName("lehrberufId")]
        public int? LehrberufId { get; set; }

        // Navigation properties
        [JsonPropertyName("rolle")]
        public Rolle? Rolle { get; set; }

        [JsonPropertyName("lehrberuf")]
        public LehrBeruf? Lehrberuf { get; set; }

        [JsonPropertyName("vollName")]
        public string VollName => $"{Vorname} {Nachname}";
    }
}
