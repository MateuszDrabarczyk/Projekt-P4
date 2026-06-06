using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PrzychodniaApp.Models;

namespace PrzychodniaApp.Views
{
    public partial class BookVisitWindow : Window
    {
        private readonly PrzychodniaDbContext _context = new PrzychodniaDbContext();
        private Pacjenci _selectedPatient;
        private readonly Wizyty _existingVisit;
        private readonly bool _isEditMode = false;

        public BookVisitWindow()
        {
            InitializeComponent();
            LoadDoctors();

            dtStartDate.SelectedDate = DateTime.Today;
            txtStartTime.Text = DateTime.Now.ToString("HH:mm");
            _isEditMode = false;
        }

        public BookVisitWindow(Wizyty visitToEdit)
        {
            InitializeComponent();
            LoadDoctors();

            _existingVisit = _context.Wizyties.Find(visitToEdit.IdWizyty);
            _isEditMode = true;
            lblHeader.Text = "Edycja Wizyty Lekarskiej";

            var doctor = _context.Lekarzes.FirstOrDefault(l => l.IdUzytkownika == _existingVisit.IdLekarza);
            if (doctor != null) cmbDoctor.SelectedItem = doctor;

            _selectedPatient = _context.Pacjencis.Find(_existingVisit.IdPacjenta);
            if (_selectedPatient != null)
            {
                txtPatientDisplay.Text = $"{_selectedPatient.Imie} {_selectedPatient.Nazwisko} (PESEL: {_selectedPatient.Pesel})";
            }

            dtStartDate.SelectedDate = _existingVisit.DataStart.Date;
            txtStartTime.Text = _existingVisit.DataStart.ToString("HH:mm");
            dtEndDate.SelectedDate = _existingVisit.DataKoniec.Date;
            txtEndTime.Text = _existingVisit.DataKoniec.ToString("HH:mm");

            txtIcd10.Text = _existingVisit.KodIcd10;
        }

        private void LoadDoctors()
        {
            cmbDoctor.ItemsSource = _context.Lekarzes.ToList();
            if (cmbDoctor.Items.Count > 0 && !_isEditMode) cmbDoctor.SelectedIndex = 0;
        }

        // Automatyczne przepisanie daty zakończenia, gdy zmienia się data rozpoczęcia
        private void dtStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtStartDate.SelectedDate.HasValue)
            {
                dtEndDate.SelectedDate = dtStartDate.SelectedDate.Value;
            }
        }

        // Walidacja
        private void txtStartTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = txtStartTime.Text.Trim();
            if (text.Length == 5)
            {
                string[] parts = text.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[0], out int hours) && int.TryParse(parts[1], out int minutes))
                {
                    if (hours >= 0 && hours < 24 && minutes >= 0 && minutes < 60)
                    {
                        //  +30 minut
                        if (!_isEditMode)
                        {
                            TimeSpan startTime = new TimeSpan(hours, minutes, 0);
                            TimeSpan endTime = startTime.Add(TimeSpan.FromMinutes(30));
                            txtEndTime.Text = endTime.ToString(@"hh\:mm");

                            if (dtStartDate.SelectedDate.HasValue)
                            {
                                dtEndDate.SelectedDate = startTime.Hours == 23 && endTime.Hours == 0
                                    ? dtStartDate.SelectedDate.Value.AddDays(1)
                                    : dtStartDate.SelectedDate.Value;
                            }
                        }
                        return;
                    }
                }
                MessageBox.Show("Wprowadzono nieprawidłową godzinę rozpoczęcia! Format to HH:mm (00-23 : 00-59).", "Błąd formatu", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtStartTime.Text = "12:00";
            }
        }

        // Walidacja poprawności wpisywanych wartości dla godziny zakończenia
        private void txtEndTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = txtEndTime.Text.Trim();
            if (text.Length == 5)
            {
                string[] parts = text.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[0], out int hours) && int.TryParse(parts[1], out int minutes))
                {
                    if (hours >= 0 && hours < 24 && minutes >= 0 && minutes < 60)
                    {
                        return; 
                    }
                }
                MessageBox.Show("Wprowadzono nieprawidłową godzinę zakończenia! Minuty nie mogą przekraczać 59, a godziny 23.", "Błąd formatu", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEndTime.Text = "12:30";
            }
        }

        private void btnSearchPatient_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditMode) return;

            var searchWin = new PatientSearchWindow { Owner = this };
            if (searchWin.ShowDialog() == true)
            {
                _selectedPatient = searchWin.SelectedPatient;
                txtPatientDisplay.Text = $"{_selectedPatient.Imie} {_selectedPatient.Nazwisko} (PESEL: {_selectedPatient.Pesel})";
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbDoctor.SelectedItem is not Lekarze selectedDoctor || _selectedPatient == null)
            {
                MessageBox.Show("Lekarz oraz Pacjent są wymagani!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!dtStartDate.SelectedDate.HasValue || !dtEndDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Wybierz datę rozpoczęcia i zakończenia!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string startDatePart = dtStartDate.SelectedDate.Value.ToString("yyyy-MM-dd");
            string endDatePart = dtEndDate.SelectedDate.Value.ToString("yyyy-MM-dd");

            string startString = $"{startDatePart} {txtStartTime.Text.Trim()}";
            if (!DateTime.TryParseExact(startString, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDateTime))
            {
                MessageBox.Show("Niepoprawny format godziny rozpoczęcia (użyj HH:mm)!", "Błąd czasu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string endString = $"{endDatePart} {txtEndTime.Text.Trim()}";
            if (!DateTime.TryParseExact(endString, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDateTime))
            {
                MessageBox.Show("Niepoprawny format godziny zakończenia (użyj HH:mm)!", "Błąd czasu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (endDateTime <= startDateTime)
            {
                MessageBox.Show("Błąd chronologii! Czas zakończenia musi nastąpić po rozpoczęciu wizyty.", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEditMode)
            {
                _existingVisit.IdLekarza = selectedDoctor.IdUzytkownika;
                _existingVisit.DataStart = startDateTime;
                _existingVisit.DataKoniec = endDateTime;
                _existingVisit.KodIcd10 = string.IsNullOrWhiteSpace(txtIcd10.Text) ? null : txtIcd10.Text;
            }
            else
            {
                var newVisit = new Wizyty
                {
                    IdPacjenta = _selectedPatient.IdPacjenta,
                    IdLekarza = selectedDoctor.IdUzytkownika,
                    DataStart = startDateTime,
                    DataKoniec = endDateTime,
                    KodIcd10 = string.IsNullOrWhiteSpace(txtIcd10.Text) ? null : txtIcd10.Text,
                    IdStatusu = 1
                };
                _context.Wizyties.Add(newVisit);
            }

            try
            {
                _context.SaveChanges();
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"BŁĄD ZAPISU (Blokada biznesowa serwera SQL):\n{ex.InnerException?.Message ?? ex.Message}",
                                "Odmowa wykonania operacji", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

    }
}