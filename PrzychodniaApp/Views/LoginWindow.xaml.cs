using System;
using System.Windows;
using PrzychodniaApp.Repositories;
using PrzychodniaApp.Models;

namespace PrzychodniaApp.Views
{
    public partial class LoginWindow : Window
    {
       
        private readonly UserRepository _userRepository = new UserRepository();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text;
            string haslo = txtHaslo.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(haslo))
            {
                MessageBox.Show("Proszę uzupełnić wszystkie pola.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
             
                var user = _userRepository.Login(login, haslo);

                if (user != null)
                {
                    
                    try
                    {
                        using (var dbContext = new PrzychodniaDbContext())
                        {
                            var loginLog = new Audyt
                            {
                                DataLogu = DateTime.Now,
                                Akcja = $"UDANE LOGOWANIE: Użytkownik [{user.Login}] zalogował się do systemu.",
                                IdUzytkownika = user.IdUzytkownika
                            };

                            dbContext.Audyts.Add(loginLog);
                            dbContext.SaveChanges();
                        }
                    }
                    catch (Exception)
                    {
                        
                    }
                   
                    MainWindow main = new MainWindow(user.IdUzytkownika, user.IdRoli);
                    main.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Błędny login lub hasło!", "Błąd logowania", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
          
                MessageBox.Show($"Błąd połączenia z bazą: {ex.Message}", "Błąd krytyczny", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void txtLogin_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
       
        }
    }
}