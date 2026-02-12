namespace MauiApp_PracticaNotas; // <--- OJO: Verifica que este sea el nombre correcto

public partial class MainPage : ContentPage
{
    const string KeyActivity = "saved_activity";
    const string KeyIsUrgent = "saved_urgent";

    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadState();
    }

    // Agregamos 'async' aquí para corregir la advertencia del DisplayAlert
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string activity = ActivityEntry.Text;
        bool isUrgent = UrgentCheckBox.IsChecked;

        Preferences.Set(KeyActivity, activity);
        Preferences.Set(KeyIsUrgent, isUrgent);

        DisplayStoredInfo(activity, isUrgent);

        // Usamos 'await' para mostrar la alerta correctamente
        await DisplayAlert("Éxito", "Estado guardado correctamente", "OK");
    }

    private void LoadState()
    {
        if (Preferences.ContainsKey(KeyActivity))
        {
            string savedActivity = Preferences.Get(KeyActivity, string.Empty);
            bool savedUrgent = Preferences.Get(KeyIsUrgent, false);

            ActivityEntry.Text = savedActivity;
            UrgentCheckBox.IsChecked = savedUrgent;

            DisplayStoredInfo(savedActivity, savedUrgent);
        }
    }

    private void DisplayStoredInfo(string act, bool urg)
    {
        string status = urg ? "URGENTE" : "Normal";
        StoredInfoLabel.Text = $"Actividad: {act}\nPrioridad: {status}";
    }
}