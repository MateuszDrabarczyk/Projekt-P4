using PrzychodniaApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace PrzychodniaApp.Repositories
{
    public class PatientRepository
    {
        private readonly PrzychodniaDbContext _context = new PrzychodniaDbContext();

        public List<Pacjenci> GetAllPatients()
        {
            // Zmieniono z Pacjencies na Pacjenci - sprawdź czy IntelliSense to łapie
            return _context.Pacjencis.ToList();
        }
        public void AddPatient(Pacjenci patient)
        {
            _context.Pacjencis.Add(patient); // Upewnij się, że nazwa 'Pacjenci' pasuje do Twojego DbContext
            _context.SaveChanges();
        }
        public void UpdatePatient(Pacjenci patient)
        {
            // EF automatycznie wyszuka obiekt po ID i zaktualizuje zmienione pola
            _context.Pacjencis.Update(patient);
            _context.SaveChanges();
        }

        public void DeletePatient(int idPacjenta)
        {
            var patient = _context.Pacjencis.Find(idPacjenta);
            if (patient != null)
            {
                _context.Pacjencis.Remove(patient);
                _context.SaveChanges();
            }
        }
    }

}