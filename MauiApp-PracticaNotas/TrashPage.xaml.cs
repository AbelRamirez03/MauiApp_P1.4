using MauiApp_PracticaNotas.Services;

namespace MauiApp_PracticaNotas
{
    public partial class TrashPage : ContentPage
    {
        public TrashPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadDeletedNotes();
        }

        private void LoadDeletedNotes()
        {
            var deletedNotes = NotesService.Instance.GetDeletedNotes();
            DeletedNotesCollectionView.ItemsSource = deletedNotes;

            bool hasNotes = deletedNotes.Any();
            EmptyLabel.IsVisible = !hasNotes;
            EmptyTrashButton.IsEnabled = hasNotes;
        }

        private async void OnRestoreNoteClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var noteId = button?.CommandParameter as string;

            if (noteId == null) return;

            bool confirm = await DisplayAlert(
                "Recuperar Nota",
                "¿Deseas recuperar esta nota?",
                "Sí",
                "No");

            if (confirm)
            {
                NotesService.Instance.RestoreNote(noteId);
                LoadDeletedNotes();
                await DisplayAlert("Éxito", "Nota recuperada correctamente", "OK");
            }
        }

        private async void OnDeletePermanentlyClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var noteId = button?.CommandParameter as string;

            if (noteId == null) return;

            bool confirm = await DisplayAlert(
                "⚠️ Eliminar Permanentemente",
                "Esta acción no se puede deshacer. ¿Estás seguro de eliminar esta nota permanentemente?",
                "Eliminar",
                "Cancelar");

            if (confirm)
            {
                NotesService.Instance.DeletePermanently(noteId);
                LoadDeletedNotes();
                await DisplayAlert("Eliminada", "La nota ha sido eliminada permanentemente", "OK");
            }
        }

        private async void OnEmptyTrashClicked(object sender, EventArgs e)
        {
            var deletedNotes = NotesService.Instance.GetDeletedNotes();

            if (!deletedNotes.Any())
            {
                await DisplayAlert("Info", "La papelera ya está vacía", "OK");
                return;
            }

            bool confirm = await DisplayAlert(
                "⚠️ Vaciar Papelera",
                $"Se eliminarán permanentemente {deletedNotes.Count} nota(s). Esta acción no se puede deshacer. ¿Continuar?",
                "Eliminar Todo",
                "Cancelar");

            if (confirm)
            {
                NotesService.Instance.ClearAllDeletedNotes();
                LoadDeletedNotes();
                await DisplayAlert("Éxito", "Papelera vaciada completamente", "OK");
            }
        }
    }
}
