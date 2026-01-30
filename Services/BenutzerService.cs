using BildungsBericht.Models;
using Microsoft.Extensions.Configuration;

namespace BildungsBericht.Services
{
    public class BenutzerService
    {
        private readonly HttpClient httpClient;
        private readonly string baseUrl;

        public BenutzerService(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7166";
        }

        public async Task<IEnumerable<CLBenutzer>> GetBenutzers()
        {
            //return await httpClient.GetFromJsonAsync<Benutzer[]>( "api/benutzers" );

            // 1️⃣ Define the URL
            string url = $"{baseUrl}/api/benutzers";

            // 2️⃣ Make the HTTP GET request
            HttpResponseMessage response = await httpClient.GetAsync( url );

            // 3️⃣ Ensure the response is successful
            response.EnsureSuccessStatusCode();

            // 4️⃣ Read and deserialize JSON into Benutzer[]
            //List<Benutzer> benutzer2;
            //try
            //{
            //    //benutzer2 = await response.Content.ReadFromJsonAsync<List<Benutzer>>();
            //    benutzer2 = await response.Content.ReadFromJsonAsync<List<Benutzer>>();
            //}
            //catch( Exception ex )
            //{
            //    benutzer2 = new List<Benutzer>();
            //}
            List<CLBenutzer> benutzers = await response.Content.ReadFromJsonAsync<List<CLBenutzer>>();

            // 5️⃣ Return the result
            return benutzers;
        }

        public async Task<bool> CreateBenutzer( Benutzer benutzer )
        {
            try
            {
                string url = $"{baseUrl}/api/benutzers";
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, benutzer );
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Benutzer aktualisieren (Update)
        public async Task<bool> UpdateBenutzer(Benutzer benutzer)
        {
            try
            {
                string url = $"{baseUrl}/api/benutzers/{benutzer.Id}";
                HttpResponseMessage response = await httpClient.PutAsJsonAsync(url, benutzer);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Benutzer löschen (Delete)
        public async Task<bool> DeleteBenutzer(int benutzerId)
        {
            try
            {
                string url = $"{baseUrl}/api/benutzers/{benutzerId}";
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
