using System.Collections.ObjectModel;
using System.Windows;
using PrzychodniaApp.Models;
using PrzychodniaApp.Repositories;

namespace PrzychodniaApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly PatientRepository _patientRepository = new PatientRepository();

        // Ta właściwość będzie "źródłem" dla DataGrid
        public ObservableCollection<Pacjenci> Patients { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            LoadPatients();
            DataContext = this; // To jest kluczowe, żeby bindowanie zadziałało!
        }

        private void LoadPatients()
        {
            var data = _patientRepository.GetAllPatients();
            Patients = new ObservableCollection<Pacjenci>(data);
        }
    }
}