using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BildungsBericht.Models
{
    public class TemplateBericht
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required]
        [JsonPropertyName("lernenderId")]
        public int LernenderId { get; set; }

        [JsonPropertyName("berufsbildnerId")]
        public int? BerufsbildnerId { get; set; }

        [Required]
        [JsonPropertyName("semester")]
        public int Semester { get; set; }

        [Required]
        [JsonPropertyName("berichtdatum")]
        public DateTime Berichtdatum { get; set; }

        [Required]
        [JsonPropertyName("erstelltDurchBenutzerId")]
        public int ErstelltDurchBenutzerId { get; set; }

        [Required]
        [JsonPropertyName("lehrberufId")]
        public int LehrberufId { get; set; }

        // Navigation properties
        [JsonPropertyName("lernender")]
        public Benutzer? Lernender { get; set; }

        [JsonPropertyName("berufsbildner")]
        public Benutzer? Berufsbildner { get; set; }

        [JsonPropertyName("erstelltDurch")]
        public Benutzer? ErstelltDurch { get; set; }

        [JsonPropertyName("lehrberuf")]
        public LehrBeruf? Lehrberuf { get; set; }
    }
}
