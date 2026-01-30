using BildungsBericht.DB;
using BildungsBericht.Models;
using Microsoft.AspNetCore.Mvc;

namespace BildungsBericht.Controllers
{
    [Route( "api/[controller]" )]
    [ApiController]
    public class BenutzersController: ControllerBase
    {
        DBHelper DBBildungsbericht;
        public BenutzersController()
        {
            String DBServerName = "DU-S-SQL-01";
            String DBName = "Bildungsbericht";
            String DBUser = "test";
            String DBPassword = "test";

            String connexionString = String.Format( "Server={0};Data Source={0};Initial Catalog={1};Database={1};User ID={2};Password={3}", 
                DBServerName, DBName, DBUser, DBPassword );
            DBBildungsbericht = new DBHelper( connexionString );

            DBBildungsbericht.ConnectionOpen();
        }


        [HttpGet]
        public async Task<ActionResult> GetBenutzers()
        {
            try
            {
                //return Ok( await BenutzerRepository.GetBenutzers() );
                return Ok( await GetBenutzersExt() );
            }
            catch( Exception )
            {
                return StatusCode( StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database" );
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetBenutzerById(int id)
        {
            try
            {
                Benutzer benutzer = await GetBenutzerByIdExt(id);
                if (benutzer != null)
                {
                    return Ok(benutzer);
                }
                else
                {
                    return NotFound(new { message = "Benutzer nicht gefunden" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Fehler beim Abrufen des Benutzers: {ex.Message}");
            }
        }

        public async Task<IEnumerable<CLBenutzer>> GetBenutzersExt()
        {
            List<CLBenutzer> listB = DBBildungsbericht.GetAllBenutzers();

            return await Task.FromResult( listB );
        }

        public async Task<Benutzer> GetBenutzerByIdExt(int id)
        {
            Benutzer benutzer = DBBildungsbericht.GetBenutzerById(id);
            return await Task.FromResult(benutzer);
        }

        [HttpPost]
        public async Task<ActionResult> CreateBenutzer([FromBody] Benutzer benutzer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                int newId = await CreateBenutzerExt(benutzer);
                
                if (newId > 0)
                {
                    return Ok(new { id = newId, message = "Benutzer erfolgreich erstellt" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Fehler beim Erstellen des Benutzers");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Fehler beim Erstellen des Benutzers: {ex.Message}");
            }
        }

        public async Task<int> CreateBenutzerExt(Benutzer benutzer)
        {
            int newId = DBBildungsbericht.CreateBenutzer(benutzer);
            return await Task.FromResult(newId);
        }

        // Benutzer aktualisieren (Update)
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBenutzer(int id, [FromBody] Benutzer benutzer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // ID muss übereinstimmen
                benutzer.Id = id;

                bool success = await UpdateBenutzerExt(benutzer);
                
                if (success)
                {
                    return Ok(new { message = "Benutzer erfolgreich aktualisiert" });
                }
                else
                {
                    return NotFound(new { message = "Benutzer nicht gefunden" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Fehler beim Aktualisieren des Benutzers: {ex.Message}");
            }
        }

        public async Task<bool> UpdateBenutzerExt(Benutzer benutzer)
        {
            bool success = DBBildungsbericht.UpdateBenutzer(benutzer);
            return await Task.FromResult(success);
        }

        // Benutzer löschen (Delete)
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBenutzer(int id)
        {
            try
            {
                bool success = await DeleteBenutzerExt(id);
                
                if (success)
                {
                    return Ok(new { message = "Benutzer erfolgreich gelöscht" });
                }
                else
                {
                    return NotFound(new { message = "Benutzer nicht gefunden" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Fehler beim Löschen des Benutzers: {ex.Message}");
            }
        }

        public async Task<bool> DeleteBenutzerExt(int id)
        {
            bool success = DBBildungsbericht.DeleteBenutzer(id);
            return await Task.FromResult(success);
        }
    }
}
