using BildungsBericht.Models;
using Microsoft.Extensions.Configuration;

namespace BildungsBericht.Services
{
    public class SelbstbewertungService
    {
        private readonly HttpClient httpClient;
        private readonly string baseUrl;

        public SelbstbewertungService(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7166";
        }

        public async Task<IEnumerable<Selbstbewertung>> GetSelbstbewertungen()
        {
            try
            {
                string url = $"{baseUrl}/api/selbstbewertung";
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                List<Selbstbewertung> selbstbewertungen = await response.Content.ReadFromJsonAsync<List<Selbstbewertung>>() ?? new List<Selbstbewertung>();
                return selbstbewertungen;
            }
            catch (Exception)
            {
                return new List<Selbstbewertung>();
            }
        }

        public async Task<bool> CreateSelbstbewertung(Selbstbewertung selbstbewertung)
        {
            try
            {
                string url = $"{baseUrl}/api/selbstbewertung";
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, selbstbewertung);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
