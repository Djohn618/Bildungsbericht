using BildungsBericht.Models;
using BildungsBericht.Services;
using Microsoft.AspNetCore.Components;

namespace BildungsBericht.Components.Pages
{
    public class SelbstbewertungBase : ComponentBase
    {
        [Inject]
        public SelbstbewertungService SelbstbewertungService { get; set; }

        public IEnumerable<Models.Selbstbewertung> Selbstbewertungen { get; set; }

        public bool ShowCreateModal { get; set; } = false;
        public bool IsCreating { get; set; } = false;
        public string? StatusMessage { get; set; }
        public bool IsError { get; set; } = false;

        public Models.Selbstbewertung NewSelbstbewertung { get; set; } = new Models.Selbstbewertung();

        protected override async Task OnInitializedAsync()
        {
            await LoadSelbstbewertungen();
        }

        protected async Task LoadSelbstbewertungen()
        {
            try
            {
                Selbstbewertungen = (await SelbstbewertungService.GetSelbstbewertungen()).ToList();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fehler beim Laden der Selbstbewertungen: {ex.Message}";
                IsError = true;
            }
        }

        protected void OpenCreateModal()
        {
            NewSelbstbewertung = new Models.Selbstbewertung
            {
                TemplateBerichtId = 1,
                SelbstNote = 4
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

        protected async Task CreateSelbstbewertung()
        {
            try
            {
                IsCreating = true;
                StatusMessage = null;
                StateHasChanged();

                bool success = await SelbstbewertungService.CreateSelbstbewertung(NewSelbstbewertung);

                if (success)
                {
                    StatusMessage = "Selbstbewertung erfolgreich erstellt!";
                    IsError = false;
                    ShowCreateModal = false;
                    await LoadSelbstbewertungen();
                }
                else
                {
                    StatusMessage = "Fehler beim Erstellen der Selbstbewertung.";
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
