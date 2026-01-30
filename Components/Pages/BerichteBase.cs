using BildungsBericht.Models;
using BildungsBericht.Services;
using Microsoft.AspNetCore.Components;

namespace BildungsBericht.Components.Pages
{
    public class BerichteBase : ComponentBase
    {
        [Inject]
        public BerichteService BerichteService { get; set; }

        public IEnumerable<TemplateBericht> Berichte { get; set; }

        public bool ShowCreateModal { get; set; } = false;
        public bool IsCreating { get; set; } = false;
        public string? StatusMessage { get; set; }
        public bool IsError { get; set; } = false;

        public Models.TemplateBericht NewBericht { get; set; } = new Models.TemplateBericht();

        protected override async Task OnInitializedAsync()
        {
            await LoadBerichte();
        }

        protected async Task LoadBerichte()
        {
            try
            {
                Berichte = (await BerichteService.GetBerichte()).ToList();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fehler beim Laden der Berichte: {ex.Message}";
                IsError = true;
            }
        }

        protected void OpenCreateModal()
        {
            NewBericht = new Models.TemplateBericht
            {
                Berichtdatum = DateTime.Today,
                Semester = 1,
                LernenderId = 1,
                ErstelltDurchBenutzerId = 1,
                LehrberufId = 1
            };
            ShowCreateModal = true;
            StatusMessage = null;
            StateHasChanged();
        }

        protected void CloseCreateModal()
        {
            ShowCreateModal = false;
            StateHasChanged();
        }

        protected async Task CreateBericht()
        {
            try
            {
                IsCreating = true;
                StatusMessage = null;
                StateHasChanged();

                bool success = await BerichteService.CreateBericht(NewBericht);

                if (success)
                {
                    StatusMessage = "Bericht erfolgreich erstellt!";
                    IsError = false;
                    ShowCreateModal = false;
                    await LoadBerichte();
                }
                else
                {
                    StatusMessage = "Fehler beim Erstellen des Berichts.";
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
                IsCreating = false;
                StateHasChanged();
            }
        }
    }
}
