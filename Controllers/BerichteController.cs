using BildungsBericht.DB;
using BildungsBericht.Models;
using Microsoft.AspNetCore.Mvc;

namespace BildungsBericht.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BerichteController : ControllerBase
    {
        DBHelper DBBildungsbericht;
        public BerichteController()
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
        public async Task<ActionResult> GetBerichte()
        {
            try
            {
                return Ok(await GetBerichteExt());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error retrieving data from the database: {ex.Message}");
            }
        }

        public async Task<IEnumerable<TemplateBericht>> GetBerichteExt()
        {
            List<TemplateBericht> listB = DBBildungsbericht.GetAllBerichte();
            return await Task.FromResult(listB);
        }

        [HttpPost]
        public async Task<ActionResult> CreateBericht([FromBody] TemplateBericht bericht)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                int newId = await CreateBerichtExt(bericht);

                if (newId > 0)
                {
                    return Ok(new { id = newId, message = "Bericht erfolgreich erstellt" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Fehler beim Erstellen des Berichts");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Fehler beim Erstellen des Berichts: {ex.Message}");
            }
        }

        public async Task<int> CreateBerichtExt(TemplateBericht bericht)
        {
            int newId = DBBildungsbericht.CreateBericht(bericht);
            return await Task.FromResult(newId);
        }
    }
}
