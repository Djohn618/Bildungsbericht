using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BildungsBericht.Models
{
    public class Selbstbewertung
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Range(1, 6)]
        [JsonPropertyName("selbstNote")]
        public int? SelbstNote { get; set; }

        [JsonPropertyName("reflexion")]
        public string? Reflexion { get; set; }

        [JsonPropertyName("gelernt")]
        public string? Gelernt { get; set; }

        [JsonPropertyName("herausforderungen")]
        public string? Herausforderungen { get; set; }

        [JsonPropertyName("naechsteZiele")]
        public string? NaechsteZiele { get; set; }

        [Required]
        [JsonPropertyName("templateBerichtId")]
        public int TemplateBerichtId { get; set; }

        // Navigation property
        [JsonPropertyName("templateBericht")]
        public TemplateBericht? TemplateBericht { get; set; }
    }
}
