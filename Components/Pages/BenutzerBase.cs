using BildungsBericht.Models;
using BildungsBericht.Services;
using Microsoft.AspNetCore.Components;

namespace BildungsBericht.Components.Pages
{
    public class BenutzerBase: ComponentBase
    {
        [Inject]
        public BenutzerService BenutzerService { get; set; }

        public IEnumerable<CLBenutzer> Benutzers { get; set; }

        public bool ShowCreateModal { get; set; } = false;
        public bool IsCreating { get; set; } = false;
        public string? StatusMessage { get; set; }
        public bool IsError { get; set; } = false;

        public Models.Benutzer NewBenutzer { get; set; } = new Models.Benutzer();

        protected override async Task OnInitializedAsync()
        {
            await LoadBenutzers();
        }

        protected async Task LoadBenutzers()
        {
            try
            {
                Benutzers = ( await BenutzerService.GetBenutzers() ).ToList();
            }
            catch( Exception ex )
            {
                StatusMessage = $"Fehler beim Laden der Benutzer: {ex.Message}";
                IsError = true;
            }
        }

        protected async Task CreateBenutzer()
        {
            try
            {
                IsCreating = true;
                StatusMessage = null;
                StateHasChanged();

                NewBenutzer.Vorname = "Bibi";
                NewBenutzer.Nachname = "Tata";
                NewBenutzer.Passwort = "P";

                bool success = await BenutzerService.CreateBenutzer( NewBenutzer );

                if( success )
                {
                    StatusMessage = "Benutzer erfolgreich erstellt!";
                    IsError = false;
                    ShowCreateModal = false;
                    await LoadBenutzers();
                }
                else
                {
                    StatusMessage = "Fehler beim Erstellen des Benutzers.";
                    IsError = true;
                }
            }
            catch( Exception ex )
            {
                StatusMessage = $"Fehler: {ex.Message}";
                IsError = true;
            }
            finally
            {
                IsCreating = false;
                StateHasChanged();
            }
        }
    }
}
