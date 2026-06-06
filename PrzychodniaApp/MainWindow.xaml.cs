using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using PrzychodniaApp.Models;
using PrzychodniaApp.Repositories;

namespace PrzychodniaApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly PatientRepository _patientRepository = new PatientRepository();

        // Ta właściwość jest źródłem danych (ItemsSource) dla Twojego DataGrid w XAML
        public ObservableCollection<Pacjenci> Patients { get; set; } = new ObservableCollection<Pacjenci>();

        public MainWindow()
        {
            InitializeComponent();
            LoadPatients();
            DataContext = this; // Kluczowe dla bindowania danych w WPF!
        }

        // Metoda pobierająca świeże dane z bazy i odświeżająca kolekcję
        private void LoadPatients()
        {
            var data = _patientRepository.GetAllPatients();

            Patients.Clear();

            foreach (var patient in data)
            {
                Patients.Add(patient);
            }
        }

        // Obsługa przycisku "DODAJ PACJENTA"
        private void btnAddPatient_Click(object sender, RoutedEventArgs e)
        {
            var addPatientWindow = new AddPatientWindow();

            if (addPatientWindow.ShowDialog() == true)
            {
                LoadPatients(); // Odśwież listę, jeśli dodano pacjenta
            }
        }

        // Obsługa kliknięcia ikony edycji (✏️) w wierszu tabeli
        private void btnEditPatient_Click(object sender, RoutedEventArgs e)
        {
            // Przechwytujemy, który przycisk został kliknięty i pobieramy dane pacjenta z tego wiersza
            if (sender is Button button && button.DataContext is Pacjenci selectedPatient)
            {
                // Otwieramy to samo okno, ale przekazujemy obiekt pacjenta do edycji przez przeciążony konstruktor
                var editWindow = new AddPatientWindow(selectedPatient);

                if (editWindow.ShowDialog() == true)
                {
                    LoadPatients(); // Odśwież listę po udanej edycji
                }
            }
        }

        // Obsługa kliknięcia ikony usuwania (❌) w wierszu tabeli
        private void btnDeletePatient_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Pacjenci selectedPatient)
            {
                // Wyświetlamy okno dialogowe z pytaniem, aby zapobiec przypadkowemu usunięciu
                var result = MessageBox.Show(
                    $"Czy na pewno chcesz usunąć pacjenta {selectedPatient.Imie} {selectedPatient.Nazwisko}?",
                    "Potwierdzenie usunięcia",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Wywołujemy usuwanie z repozytorium
                        _patientRepository.DeletePatient(selectedPatient.IdPacjenta);
                        LoadPatients(); // Odświeżamy tabelę
                    }
                    catch (Exception ex)
                    {
                        // Zabezpieczenie przed usunięciem pacjenta posiadającego powiązane rekordy w bazie (np. wizyty)
                        MessageBox.Show(
                            $"Nie można usunąć pacjenta. Prawdopodobnie posiada on historię wizyt w systemie.\n\nSzczegóły błędu: {ex.Message}",
                            "Błąd bazy danych",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
                }
            }
        }
    }
}