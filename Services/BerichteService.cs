using BildungsBericht.Models;
using Microsoft.Extensions.Configuration;

namespace BildungsBericht.Services
{
    public class BerichteService
    {
        private readonly HttpClient httpClient;
        private readonly string baseUrl;

        public BerichteService(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7166";
        }

        public async Task<IEnumerable<TemplateBericht>> GetBerichte()
        {
            try
            {
                string url = $"{baseUrl}/api/berichte";
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                List<TemplateBericht> berichte = await response.Content.ReadFromJsonAsync<List<TemplateBericht>>() ?? new List<TemplateBericht>();
                return berichte;
            }
            catch (Exception)
            {
                return new List<TemplateBericht>();
            }
        }

        public async Task<bool> CreateBericht(TemplateBericht bericht)
        {
            try
            {
                string url = $"{baseUrl}/api/berichte";
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, bericht);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Bericht aktualisieren (Update)
        public async Task<bool> UpdateBericht(TemplateBericht bericht)
        {
            try
            {
                string url = $"{baseUrl}/api/berichte/{bericht.Id}";
                HttpResponseMessage response = await httpClient.PutAsJsonAsync(url, bericht);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Bericht löschen (Delete)
        public async Task<bool> DeleteBericht(int berichtId)
        {
            try
            {
                string url = $"{baseUrl}/api/berichte/{berichtId}";
                HttpResponseMessage response = await httpClient.DeleteAsync(url);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
