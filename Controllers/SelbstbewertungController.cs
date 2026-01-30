using BildungsBericht.DB;
using BildungsBericht.Models;
using Microsoft.AspNetCore.Mvc;

namespace BildungsBericht.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SelbstbewertungController : ControllerBase
    {
        DBHelper DBBildungsbericht;
        public SelbstbewertungController()
        {
            String DBServerName = "DU-S-SQL-01";
            String DBName = "Bildungsbericht";
            String DBUser = "test";
            String DBPassword = "test";

            String connexionString = String.Format("Server={0};Data Source={0};Initial Catalog={1};Database={1};User ID={2};Password={3}",
                DBServerName, DBName, DBUser, DBPassword);
            DBBildungsbericht = new DBHelper(connexionString);

            DBBildungsbericht.ConnectionOpen();
        }

        [HttpGet]
        public async Task<ActionResult> GetSelbstbewertungen()
        {
            try
            {
                return Ok(await GetSelbstbewertungenExt());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error retrieving data from the database: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Selbstbewertung>> GetSelbstbewertungenExt()
        {
            List<Selbstbewertung> list = DBBildungsbericht.GetAllSelbstbewertungen();
            return await Task.FromResult(list);
        }

        [HttpPost]
        public async Task<ActionResult> CreateSelbstbewertung([FromBody] Selbstbewertung selbstbewertung)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                int newId = await CreateSelbstbewertungExt(selbstbewertung);

                if (newId > 0)
                {
                    return Ok(new { id = newId, message = "Selbstbewertung erfolgreich erstellt" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Fehler beim Erstellen der Selbstbewertung");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Fehler beim Erstellen der Selbstbewertung: {ex.Message}");
            }
        }

        public async Task<int> CreateSelbstbewertungExt(Selbstbewertung selbstbewertung)
        {
            int newId = DBBildungsbericht.CreateSelbstbewertung(selbstbewertung);
            return await Task.FromResult(newId);
        }

        // Selbstbewertung aktualisieren (Update)
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateSelbstbewertung(int id, [FromBody] Selbstbewertung selbstbewertung)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // ID muss übereinstimmen
                selbstbewertung.Id = id;

                bool success = await UpdateSelbstbewertungExt(selbstbewertung);
                
                if (success)
                {
                    return Ok(new { message = "Selbstbewertung erfolgreich aktualisiert" });
                }
                else
                {
                    return NotFound(new { message = "Selbstbewertung nicht gefunden" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Fehler beim Aktualisieren der Selbstbewertung: {ex.Message}");
            }
        }

        public async Task<bool> UpdateSelbstbewertungExt(Selbstbewertung selbstbewertung)
        {
            bool success = DBBildungsbericht.UpdateSelbstbewertung(selbstbewertung);
            return await Task.FromResult(success);
        }

        // Selbstbewertung löschen (Delete)
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSelbstbewertung(int id)
        {
            try
            {
                bool success = await DeleteSelbstbewertungExt(id);
                
                if (success)
                {
                    return Ok(new { message = "Selbstbewertung erfolgreich gelöscht" });
                }
                else
                {
                    return NotFound(new { message = "Selbstbewertung nicht gefunden" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Fehler beim Löschen der Selbstbewertung: {ex.Message}");
            }
        }

        public async Task<bool> DeleteSelbstbewertungExt(int id)
        {
            bool success = DBBildungsbericht.DeleteSelbstbewertung(id);
            return await Task.FromResult(success);
        }
    }
}
