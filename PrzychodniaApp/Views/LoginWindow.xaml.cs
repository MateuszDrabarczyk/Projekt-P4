using System;
using System.Windows;
using PrzychodniaApp.Repositories;
using PrzychodniaApp.Models;

namespace PrzychodniaApp.Views
{
    public partial class LoginWindow : Window
    {
        // Pole zadeklarowane poprawnie na poziomie klasy
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
                // Wywołanie logiki z Twojego UserRepository
                var user = _userRepository.Login(login, haslo);

                if (user != null)
                {
                    // Jeśli logowanie się uda, otwieramy MainWindow
                    MainWindow main = new MainWindow();
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
                // Wyświetla błąd, jeśli np. baza danych nie jest włączona
                MessageBox.Show($"Błąd połączenia z bazą: {ex.Message}", "Błąd krytyczny", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void txtLogin_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}