using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using PrzychodniaApp.Models;
using PrzychodniaApp.Repositories;

namespace PrzychodniaApp.Views
{
    public partial class AddPatientWindow : Window
    {
        private readonly PatientRepository _patientRepository = new PatientRepository();
        private readonly Pacjenci _existingPatient;
        private readonly bool _isEditMode = false;  

     
        public AddPatientWindow()
        {
            InitializeComponent();
            _isEditMode = false;
        }

     
        public AddPatientWindow(Pacjenci patientToEdit)
        {
            InitializeComponent();
            _existingPatient = patientToEdit;
            _isEditMode = true;

           
            txtPesel.Text = _existingPatient.Pesel;
            txtImie.Text = _existingPatient.Imie;
            txtNazwisko.Text = _existingPatient.Nazwisko;
            txtTelefon.Text = _existingPatient.Telefon;
            txtAdres.Text = _existingPatient.Adres;

            
        }

        // Blokada wpisywania liter 
        private void OnlyNumbers_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // Logika przycisku Zapisz 
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Walidacja pustych pól
            if (string.IsNullOrWhiteSpace(txtPesel.Text) ||
                string.IsNullOrWhiteSpace(txtImie.Text) ||
                string.IsNullOrWhiteSpace(txtNazwisko.Text))
            {
                MessageBox.Show("Pola: PESEL, Imię i Nazwisko są wymagane!", "Ostrzeżenie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Walidacja długości PESEL
            if (txtPesel.Text.Length != 11)
            {
                MessageBox.Show("Numer PESEL bazy musi składać się z dokładnie 11 cyfr!", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Walidacja długości telefonu 
            if (!string.IsNullOrWhiteSpace(txtTelefon.Text) && txtTelefon.Text.Length != 9)
            {
                MessageBox.Show("Numer telefonu must składać się z dokładnie 9 cyfr!", "Błąd walidacji", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEditMode)
            {    _existingPatient.Pesel = txtPesel.Text;
                _existingPatient.Imie = txtImie.Text;
                _existingPatient.Nazwisko = txtNazwisko.Text;
                _existingPatient.Telefon = txtTelefon.Text;
                _existingPatient.Adres = txtAdres.Text;

                try
                {
                    _patientRepository.UpdatePatient(_existingPatient);
                    this.DialogResult = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas edycji danych: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
             
                var newPatient = new Pacjenci
                {
                    Pesel = txtPesel.Text,
                    Imie = txtImie.Text,
                    Nazwisko = txtNazwisko.Text,
                    Telefon = txtTelefon.Text,
                    Adres = txtAdres.Text
                };

                try
                {
                    _patientRepository.AddPatient(newPatient);
                    this.DialogResult = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas dodawania pacjenta: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}