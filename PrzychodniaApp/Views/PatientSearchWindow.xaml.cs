using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PrzychodniaApp.Models;

namespace PrzychodniaApp.Views
{
    public partial class PatientSearchWindow : Window
    {
        private readonly PrzychodniaDbContext _context = new PrzychodniaDbContext();
        private List<Pacjenci> _allPatients;
        public Pacjenci SelectedPatient { get; private set; }

        public PatientSearchWindow()
        {
            InitializeComponent();
            LoadPatients();
        }

        private void LoadPatients()
        {
            _allPatients = _context.Pacjencis.ToList();
            dgPatientsSearch.ItemsSource = _allPatients;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = txtSearch.Text.ToLower();
            if (string.IsNullOrWhiteSpace(filter))
            {
                dgPatientsSearch.ItemsSource = _allPatients;
            }
            else
            {
                dgPatientsSearch.ItemsSource = _allPatients.Where(p =>
                    p.Nazwisko.ToLower().Contains(filter) ||
                    p.Imie.ToLower().Contains(filter) ||
                    p.Pesel.Contains(filter)).ToList();
            }
        }

        private void ConfirmSelection()
        {
            if (dgPatientsSearch.SelectedItem is Pacjenci patient)
            {
                SelectedPatient = patient;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e) => ConfirmSelection();
        private void dgPatientsSearch_MouseDoubleClick(object sender, MouseButtonEventArgs e) => ConfirmSelection();
    }
}