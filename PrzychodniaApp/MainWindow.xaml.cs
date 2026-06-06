using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PrzychodniaApp.Models;

namespace PrzychodniaApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly PrzychodniaDbContext _context = new PrzychodniaDbContext();

        private readonly int _currentUserId;
        private readonly int _currentUserRole;

        public ObservableCollection<Pacjenci> Patients { get; set; } = new ObservableCollection<Pacjenci>();
        public ObservableCollection<Wizyty> Visits { get; set; } = new ObservableCollection<Wizyty>();
        public ObservableCollection<Audyt> AuditLogs { get; set; } = new ObservableCollection<Audyt>();

      
        public MainWindow(int userId, int roleId)
        {
            InitializeComponent();
            _currentUserId = userId;
            _currentUserRole = roleId;

            LoadAllData();
            ApplySecurityRules();
            LoadLoggedUserInformation(); 

            DataContext = this;
        }

        private void LoadAllData()
        {
            try
            {
                // 1. Ładowanie Pacjentów
                var patientsData = _context.Pacjencis.ToList();
                Patients.Clear();
                foreach (var p in patientsData) Patients.Add(p);

                // 2. Ładowanie Wizyt
                var visitsData = _context.Wizyties.ToList();
                Visits.Clear();
                foreach (var v in visitsData) Visits.Add(v);

                // 3. Ładowanie Audytu 
                if (_currentUserRole == 3)
                {
                    var auditData = _context.Audyts.OrderByDescending(a => a.DataLogu).ToList();
                    AuditLogs.Clear();
                    foreach (var a in auditData) AuditLogs.Add(a);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd pobierania danych z bazy: {ex.Message}", "Błąd krytyczny", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadLoggedUserInformation()
        {
            try
            {
              
                var user = _context.Uzytkownicies.FirstOrDefault(u => u.IdUzytkownika == _currentUserId);
                if (user != null)
                {
                    string rolaNazwa = _currentUserRole == 3 ? "Administrator" :
                                       _currentUserRole == 2 ? "Recepcjonista" : "Lekarz";

                    txtLoggedUser.Text = $"Zalogowano jako: {rolaNazwa} ({user.Login.Trim()})";
                }
            }
            catch
            {
                txtLoggedUser.Text = "Zalogowano w trybie offline";
            }
        }

        private void ApplySecurityRules()
        {
            
            if (_currentUserRole == 3) // Administrator
            {
                tabAdminAudit.Visibility = Visibility.Visible; // Pokazujemy logi RODO
            }
            else if (_currentUserRole == 2) // Recepcjonista
            {
                btnBookVisit.IsEnabled = false;
                btnBookVisit.ToolTip = "Tylko personel medyczny może zarządzać harmonogramem wizyt.";
            }
        }

        
        private void btnAddPatient_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddPatientWindow();
            if (addWindow.ShowDialog() == true) LoadAllData();
        }

        private void btnEditPatient_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Pacjenci selected)
            {
                var editWindow = new AddPatientWindow(selected);
                if (editWindow.ShowDialog() == true) LoadAllData();
            }
        }

        private void btnDeletePatient_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Pacjenci selected)
            {
                var result = MessageBox.Show($"Usunąć pacjenta {selected.Imie} {selected.Nazwisko}?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                     
                        bool hasVisits = _context.Wizyties.Any(w => w.IdPacjenta == selected.IdPacjenta);
                        if (hasVisits)
                        {
                            MessageBox.Show($"Nie można usunąć pacjenta {selected.Imie} {selected.Nazwisko}, ponieważ posiada on aktywne wizyty lekarskie w harmonogramie!\n\nAnuluj najpierw jego wizyty.",
                                            "Błąd spójności danych", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        _context.Pacjencis.Remove(selected);
                        _context.SaveChanges();
                        LoadAllData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Błąd integralności danych: {ex.Message}", "Błąd usuwania", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        // MODUŁ WIZYT 
        private void btnBookVisit_Click(object sender, RoutedEventArgs e)
        {
            var bookWindow = new BookVisitWindow { Owner = this };
            if (bookWindow.ShowDialog() == true)
            {
                LoadAllData();
                MessageBox.Show("Wizyta została pomyślnie zarejestrowana w bazie danych.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void dgVisits_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is DataGrid dataGrid && dataGrid.SelectedItem is Wizyty selectedVisit)
            {
                var targetPatient = Patients.FirstOrDefault(p => p.IdPacjenta == selectedVisit.IdPacjenta);
                if (targetPatient != null)
                {
                    mainTabControl.SelectedIndex = 0;
                    dgPatients.SelectedItem = targetPatient;
                    dgPatients.ScrollIntoView(targetPatient);
                }
            }
        }

        private void btnEditVisit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Wizyty selectedVisit)
            {
                var editWindow = new BookVisitWindow(selectedVisit) { Owner = this };
                if (editWindow.ShowDialog() == true)
                {
                    LoadAllData();
                }
            }
        }

        private void btnDeleteVisit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Wizyty selectedVisit)
            {
                var result = MessageBox.Show("Czy na pewno chcesz usunąć tę wizytę z harmonogramu?",
                                             "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Wizyties.Remove(selectedVisit);
                        _context.SaveChanges();
                        LoadAllData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Błąd usuwania: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var confirmation = MessageBox.Show("Czy na pewno chcesz się wylogować i wrócić do ekranu logowania?",
                                               "Wylogowanie z systemu", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirmation == MessageBoxResult.Yes)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}