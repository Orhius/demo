using shoesMic.Services;
using System.Windows;
using System.Windows.Controls;

namespace shoesMic.Views
{
    /// <summary>
    /// Окно авторизации пользователя.
    /// Предоставляет форму входа по логину/паролю, а также вход в режиме гостя.
    /// </summary>
    public partial class LoginWindow : Window
    {
        // ──────────────────────────────────────────────
        //  Поля
        // ──────────────────────────────────────────────

        /// <summary>Сервис для работы с базой данных.</summary>
        private readonly DatabaseService _db = new DatabaseService(
            host: "localhost",
            port: 5432,
            database: "shoes",
            username: "orhius",
            password: "1234"
        );

        // ──────────────────────────────────────────────
        //  Конструктор
        // ──────────────────────────────────────────────

        public LoginWindow()
        {
            InitializeComponent();
        }

        // ──────────────────────────────────────────────
        //  Обработчики событий
        // ──────────────────────────────────────────────

        /// <summary>
        /// Обрабатывает нажатие кнопки «Войти».
        /// Проверяет логин и пароль через сервис БД.
        /// При успехе открывает окно каталога товаров.
        /// </summary>
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTextBox.Text;
            var password = PasswordBox.Password;

            var user = _db.AuthenticateUser(login, password);

            if (user.HasValue)
            {
                // Открываем каталог с данными авторизованного пользователя
                var productsWindow = new ProductsWindow(user.Value.userId, user.Value.fullName, user.Value.role, _db);
                productsWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки «Войти как гость».
        /// Открывает каталог без прав администратора.
        /// </summary>
        private void Guest_Click(object sender, RoutedEventArgs e)
        {
            var productsWindow = new ProductsWindow(null, "Гость", "Гость", _db);
            productsWindow.Show();
            this.Close();
        }
    }
}

