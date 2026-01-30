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

        // Edit-Funktionalität
        public bool ShowEditModal { get; set; } = false;
        public bool IsEditing { get; set; } = false;
        public Models.Benutzer EditBenutzer { get; set; } = new Models.Benutzer();

        // Delete-Funktionalität
        public bool ShowDeleteModal { get; set; } = false;
        public bool IsDeleting { get; set; } = false;
        public CLBenutzer DeleteBenutzer { get; set; }

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

                // Benutzer erstellen mit den Formulardaten
                bool success = await BenutzerService.CreateBenutzer( NewBenutzer );

                if( success )
                {
                    StatusMessage = "Benutzer erfolgreich erstellt!";
                    IsError = false;
                    ShowCreateModal = false;
                    // Formular zurücksetzen
                    NewBenutzer = new Models.Benutzer();
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

        // Modal öffnen für neuen Benutzer
        protected void OpenCreateModal()
        {
            // Neues Benutzer-Objekt mit Standardwerten
            NewBenutzer = new Models.Benutzer
            {
                RolleId = 1, // Standard Rolle
                Geburtsdatum = DateTime.Today.AddYears(-18) // Standard Geburtsdatum
            };
            ShowCreateModal = true;
            StatusMessage = null;
            StateHasChanged();
        }

        // Modal schließen
        protected void CloseCreateModal()
        {
            ShowCreateModal = false;
            StateHasChanged();
        }

        // Edit-Modal öffnen
        protected void OpenEditModal(CLBenutzer benutzer)
        {
            EditBenutzer = new Models.Benutzer
            {
                Id = benutzer.BenutzerId,
                Vorname = benutzer.FirstName,
                Nachname = benutzer.LastName,
                RolleId = 1, // Standard Rolle
                Geburtsdatum = DateTime.Today.AddYears(-18),
                Passwort = "******" // Platzhalter für Passwort
            };
            ShowEditModal = true;
            StatusMessage = null;
            StateHasChanged();
        }

        // Edit-Modal schließen
        protected void CloseEditModal()
        {
            ShowEditModal = false;
            StateHasChanged();
        }

        // Benutzer aktualisieren
        protected async Task UpdateBenutzer()
        {
            try
            {
                IsEditing = true;
                StatusMessage = null;
                StateHasChanged();

                bool success = await BenutzerService.UpdateBenutzer(EditBenutzer);

                if (success)
                {
                    StatusMessage = "Benutzer erfolgreich aktualisiert!";
                    IsError = false;
                    ShowEditModal = false;
                    await LoadBenutzers();
                }
                else
                {
                    StatusMessage = "Fehler beim Aktualisieren des Benutzers.";
                    IsError = true;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fehler: {ex.Message}";
                IsError = true;
            }
            finally
            {
                IsEditing = false;
                StateHasChanged();
            }
        }

        // Delete-Modal öffnen
        protected void OpenDeleteModal(CLBenutzer benutzer)
        {
            DeleteBenutzer = benutzer;
            ShowDeleteModal = true;
            StatusMessage = null;
            StateHasChanged();
        }

        // Delete-Modal schließen
        protected void CloseDeleteModal()
        {
            ShowDeleteModal = false;
            StateHasChanged();
        }

        // Benutzer löschen
        protected async Task ConfirmDeleteBenutzer()
        {
            try
            {
                IsDeleting = true;
                StatusMessage = null;
                StateHasChanged();

                bool success = await BenutzerService.DeleteBenutzer(DeleteBenutzer.BenutzerId);

                if (success)
                {
                    StatusMessage = "Benutzer erfolgreich gelöscht!";
                    IsError = false;
                    ShowDeleteModal = false;
                    await LoadBenutzers();
                }
                else
                {
                    StatusMessage = "Fehler beim Löschen des Benutzers.";
                    IsError = true;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fehler: {ex.Message}";
                IsError = true;
            }
            finally
            {
                IsDeleting = false;
                StateHasChanged();
            }
        }
    }
}
