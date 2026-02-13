using DocumentFormat.OpenXml.Drawing.Charts;
using MauiApp_PracticaNotas.Models;
using MauiApp_PracticaNotas.Services;

namespace MauiApp_PracticaNotas
{
    public partial class MainPage : ContentPage
    {
        private bool isMenuOpen = false;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadNotes();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            string activity = ActivityEntry.Text;

            if (string.IsNullOrWhiteSpace(activity))
            {
                await DisplayAlert("Error", "Por favor escribe una actividad", "OK");
                return;
            }

            bool isUrgent = UrgentCheckBox.IsChecked;

            var note = new Note
            {
                Activity = activity,
                IsUrgent = isUrgent
            };

            NotesService.Instance.AddOrUpdateNote(note);

            // Limpiar campos
            ActivityEntry.Text = string.Empty;
            UrgentCheckBox.IsChecked = false;

            await DisplayAlert("Éxito", "Nota guardada correctamente", "OK");

            LoadNotes();
        }

        private void LoadNotes()
        {
            var activeNotes = NotesService.Instance.GetActiveNotes();
            NotesCollectionView.ItemsSource = activeNotes;
            EmptyLabel.IsVisible = !activeNotes.Any();
        }

        private async void OnDeleteNoteClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var noteId = button?.CommandParameter as string;

            if (noteId == null) return;

            bool confirm = await DisplayAlert(
                "Eliminar Nota",
                "¿Deseas mover esta nota a la papelera?",
                "Sí",
                "No");

            if (confirm)
            {
                NotesService.Instance.MoveToTrash(noteId);
                LoadNotes();
                await DisplayAlert("Éxito", "Nota movida a la papelera", "OK");
            }
        }

        private async void OnMenuClicked(object sender, EventArgs e)
        {
            if (isMenuOpen)
            {
                await CloseMenu();
            }
            else
            {
                await OpenMenu();
            }
        }

        private async Task OpenMenu()
        {
            isMenuOpen = true;
            Overlay.IsVisible = true;
            Overlay.InputTransparent = false;

            var tasks = new List<Task>
            {
                SideMenu.TranslateTo(0, 0, 250, Easing.CubicOut),
                Overlay.FadeTo(0.5, 250)
            };

            await Task.WhenAll(tasks);
        }

        private async Task CloseMenu()
        {
            isMenuOpen = false;

            var tasks = new List<Task>
            {
                SideMenu.TranslateTo(-250, 0, 250, Easing.CubicIn),
                Overlay.FadeTo(0, 250)
            };

            await Task.WhenAll(tasks);

            Overlay.IsVisible = false;
            Overlay.InputTransparent = true;
        }

        private async void OnOverlayTapped(object sender, EventArgs e)
        {
            await CloseMenu();
        }

        private async void OnActiveNotesClicked(object sender, EventArgs e)
        {
            await CloseMenu();
            // Ya estamos en la página principal
        }

        private async void OnTrashClicked(object sender, EventArgs e)
        {
            await CloseMenu();
            await Navigation.PushAsync(new TrashPage());
        }
    }

    // Convertidor para el color de fondo según urgencia
    public class UrgentColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool isUrgent)
            {
                return isUrgent ? Color.FromArgb("#E74C3C") : Color.FromArgb("#3498DB");
            }
            return Color.FromArgb("#3498DB");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Convertidor para el texto de urgencia
    public class UrgentTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool isUrgent)
            {
                return isUrgent ? "URGENTE" : "Normal";
            }
            return "Normal";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
