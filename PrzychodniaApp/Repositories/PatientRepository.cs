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
    }
}