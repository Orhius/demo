using System.Windows;
using System.Windows.Controls;
using shoesMic.Services;

namespace shoesMic
{
    public partial class LoginWindow : Window
    {
        private readonly DatabaseService db = new DatabaseService(
            host: "localhost",
            port: 5432,
            database: "shoes",
            username: "orhius",
            password: "1234"
        );

        public LoginWindow() { InitializeComponent(); }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTextBox.Text;
            var password = PasswordBox.Password;

            var user = db.AuthenticateUser(login, password);

            if (user.HasValue)
            {
                var productsWindow = new ProductsWindow(user.Value.userId, user.Value.fullName, user.Value.role, db);
                productsWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Guest_Click(object sender, RoutedEventArgs e)
        {
            var productsWindow = new ProductsWindow(null, "Гость", "Гость", db);
            productsWindow.Show();
            this.Close();
        }
    }
}
