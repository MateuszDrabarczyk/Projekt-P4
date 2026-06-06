using PrzychodniaApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace PrzychodniaApp.Repositories
{
    public class UserRepository
    {
        private readonly PrzychodniaDbContext _context;

        public UserRepository()
        {
            _context = new PrzychodniaDbContext();
        }

  
        public Uzytkownicy Login(string login, string haslo)
        {
            
            return _context.Uzytkownicies
                .FirstOrDefault(u => u.Login == login && u.HasloHash == haslo);
        }
    }
}